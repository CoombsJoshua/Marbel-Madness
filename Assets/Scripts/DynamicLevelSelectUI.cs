using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OriginLabs;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class DynamicLevelSelectUI : Menu_
{
    [Header("UI References")]
    public GameObject levelButtonPrefab; // Prefab for individual level buttons
    public Transform levelsGrid; // Grid where level buttons will be instantiated
    public TextMeshProUGUI chapterText; // Text displaying the current chapter
    public Button nextChapterButton;
    public Button previousChapterButton;

    [Header("Level Data")]
    public int levelsPerChapter = 10;
    public int totalChapters = 5;

    private int currentChapter = 1;
    private string pendingLevelName; // Temporary field to store the level name for delayed connection

    public override MenuType MenuType => MenuType.LevelSelect;

    public TMP_InputField m_MovementSpeed, m_MaxVelocity, m_JumpPower;
    public Toggle m_DisplayNames;
public void ApplySettings()
{
    // Parse input fields to floats and bool
    float movementSpeed = float.TryParse(m_MovementSpeed.text, out var ms) ? ms : SettingsManager.Instance.movementSpeed;
    float maxVelocity = float.TryParse(m_MaxVelocity.text, out var mv) ? mv : SettingsManager.Instance.maxVelocity;
    float jumpPower = float.TryParse(m_JumpPower.text, out var jp) ? jp : SettingsManager.Instance.jumpPower;
    bool displayNames = m_DisplayNames.isOn;

    // Update settings
    SettingsManager.Instance.UpdateSettings(movementSpeed, maxVelocity, jumpPower, displayNames);

    Debug.Log("Settings applied.");
}
    void Start()
    {
        UpdateUI();

                // Initialize input fields with default values from SettingsManager
        m_MovementSpeed.text = SettingsManager.Instance.movementSpeed.ToString("F2");
        m_MaxVelocity.text = SettingsManager.Instance.maxVelocity.ToString("F2");
        m_JumpPower.text = SettingsManager.Instance.jumpPower.ToString("F2");
        m_DisplayNames.isOn = SettingsManager.Instance.displayNames;
    }

    public void NextChapter()
    {
        if (currentChapter < totalChapters)
        {
            currentChapter++;
            UpdateUI();
        }
    }

    public void PreviousChapter()
    {
        if (currentChapter > 1)
        {
            currentChapter--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Update chapter text
        chapterText.text = $"Chapter {currentChapter}";

        // Clear existing level buttons
        foreach (Transform child in levelsGrid)
        {
            Destroy(child.gameObject);
        }

        // Populate levels for the current chapter
        for (int i = 1; i <= levelsPerChapter; i++)
        {
            int levelIndex = (currentChapter - 1) * levelsPerChapter + i;
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsGrid);
            LevelButton buttonScript = levelButton.GetComponent<LevelButton>();

            // Configure the button
         //   int stars = ProgressionManager.Instance.GetStarsForLevel(levelIndex);
            bool isUnlocked = ProgressionManager.Instance.IsLevelUnlocked(levelIndex);
buttonScript.Configure(levelIndex,
    ProgressionManager.Instance.IsLevelUnlocked(levelIndex), 
    ConnectToLevel);

        }

        // Update navigation buttons
        nextChapterButton.interactable = currentChapter < totalChapters;
        previousChapterButton.interactable = currentChapter > 1;
    }
public void ConnectToLevel(int levelIndex)
{
    // if (!ProgressionManager.Instance.IsLevelUnlocked(levelIndex))
    // {
    //     Debug.LogWarning("Level is locked!");
    //     return;
    // }
    Debug.Log("Connecting to:: " + levelIndex);
    var player = NetworkManager.Singleton.LocalClient.PlayerObject;
    if (player != null)
    {
        Userinterface.Instance.m_CanvasManager.SwitchCanvas(MenuType.Teleporting);
        // Teleport the player to the selected level
        LevelManager.Instance.TeleportPlayerToLevel(player.gameObject, levelIndex - 1);
    }
    else
    {
        Debug.LogError("Player object not found!");
    }

    
}


    private void ConnectToPendingServer()
    {
        if (!string.IsNullOrEmpty(pendingLevelName))
        {
            ConnectToNewServer(pendingLevelName);
            pendingLevelName = null; // Clear the pending level name
        }
    }

    private void ConnectToNewServer(string levelName)
    {
        var serverInfo = MatchmakingServer.Instance.GetServerForLevel(levelName);

        if (serverInfo != null)
        {
            Debug.Log($"Connecting to {serverInfo.IpAddress}:{serverInfo.Port}");

            // Set connection data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                serverInfo.IpAddress,
                (ushort)serverInfo.Port
            );

            // Start client connection
            NetworkManager.Singleton.StartClient();

            // Add callback for successful connection
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                Debug.Log($"Successfully connected to {levelName}");
                Userinterface.Instance.OpenUI(OriginLabs.MenuType.GameUI);
            };

            // Add callback for connection failure
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
            {
                Debug.LogError("Failed to connect to server.");
            };
        }
        else
        {
            Debug.LogWarning("No available server for the selected level.");
        }
    }
}
