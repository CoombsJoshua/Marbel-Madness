
using OriginLabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Userinterface : MonoBehaviour
{
    [SerializeField] public CanvasManager m_CanvasManager;

    // Singleton
    public static Userinterface Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        Invoke("mainstart", 0.25f);
    }

    void mainstart(){
        m_CanvasManager.SwitchCanvas(MenuType.MainMenu);
    }

    void Update()
    {
        if (m_CanvasManager != null)
        {
            // Check if the active canvas is LevelSelect or Teleporting
            Player_Runtime.IsInMenu = 
                m_CanvasManager.activeCanvasType == MenuType.LevelSelect ||
                m_CanvasManager.activeCanvasType == MenuType.Teleporting ||
                m_CanvasManager.activeCanvasType == MenuType.MarbleMenu;
        }
        else
        {
            UnityEngine.Debug.LogWarning("CanvasManager is not assigned in Userinterface.");
        }
    if (Input.GetKeyDown(KeyCode.Escape) && Player_Runtime.IsInSafeArea)
    {
        // Check the current active canvas type
        if (m_CanvasManager.activeCanvasType == MenuType.LevelSelect)
        {
            // If LevelSelect is open, switch to GameUI
            m_CanvasManager.SwitchCanvas(MenuType.GameUI);
        }
        else
        {
            // Otherwise, open LevelSelect
            m_CanvasManager.SwitchCanvas(MenuType.LevelSelect);
        }
    }
    }

    public void OpenUI(MenuType type)
    {
        if (m_CanvasManager != null)
        {
            m_CanvasManager.SwitchCanvas(type);
        }
        else
        {
            UnityEngine.Debug.LogError("CanvasManager is not assigned in Userinterface.");
        }
    }

    public void OpenMarbleMenu(){
        m_CanvasManager.SwitchCanvas(MenuType.MarbleMenu);
    }
    public void OpenMainMenu(){
        m_CanvasManager.SwitchCanvas(MenuType.MainMenu);
    }

    #region  Marble

        [Header("References")]
    public Renderer marbleRenderer; // Reference to the marble's renderer
    public MarbleCosmeticsDatabase cosmeticsDatabase; // Reference to the cosmetics database

    private int currentCosmeticID; // Tracks the currently applied cosmetic


    public void ApplyCosmetic(int cosmeticID)
    {
        if(marbleRenderer== null) return;
        MarbleCosmetic cosmetic = cosmeticsDatabase.Cosmetics.Find(c => c.ID == cosmeticID);

        if (cosmetic != null)
        {
            currentCosmeticID = cosmeticID;
            marbleRenderer.material.mainTexture = cosmetic.Texture;
            Debug.Log($"Applied cosmetic: {cosmetic.Name}");
        }
        else
        {
            Debug.LogWarning($"Cosmetic with ID {cosmeticID} not found in the database.");
        }
    }

    public MarbleCosmetic GetCurrentCosmetic()
    {
        return cosmeticsDatabase.Cosmetics.Find(c => c.ID == currentCosmeticID);
    }
    #endregion
}
