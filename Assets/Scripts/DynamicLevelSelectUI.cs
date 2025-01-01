using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DynamicLevelSelectUI : MonoBehaviour
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

    void Start()
    {
        UpdateUI();
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
            int stars = ProgressionManager.Instance.GetStarsForLevel(levelIndex);
            bool isUnlocked = ProgressionManager.Instance.IsLevelUnlocked(levelIndex);

            buttonScript.Configure(levelIndex, stars, isUnlocked, LoadLevel);
        }

        // Update navigation buttons
        nextChapterButton.interactable = currentChapter < totalChapters;
        previousChapterButton.interactable = currentChapter > 1;
    }

    public void LoadLevel(int levelIndex)
    {
        if (ProgressionManager.Instance.IsLevelUnlocked(levelIndex))
        {
            SceneManager.LoadScene($"Level{levelIndex}");
        }
    }
}
