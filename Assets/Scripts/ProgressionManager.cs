using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    private int[] starsPerLevel; // Array to store stars per level
    private int highestUnlockedLevel = 1;

    void Awake()
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

        LoadProgress();
    }

    public void SetStarsForLevel(int levelIndex, int stars)
    {
        starsPerLevel[levelIndex - 1] = Mathf.Clamp(stars, 0, 3);
        SaveProgress();
    }

    public int GetStarsForLevel(int levelIndex)
    {
        return starsPerLevel[levelIndex - 1];
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= highestUnlockedLevel;
    }

    public void UnlockNextLevel()
    {
        if (highestUnlockedLevel < starsPerLevel.Length)
        {
            highestUnlockedLevel++;
            SaveProgress();
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("HighestUnlockedLevel", highestUnlockedLevel);
        for (int i = 0; i < starsPerLevel.Length; i++)
        {
            PlayerPrefs.SetInt($"Level{(i + 1)}Stars", starsPerLevel[i]);
        }
    }

    private void LoadProgress()
    {
        highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);
        starsPerLevel = new int[20]; // Assuming `totalLevels` is predefined

        for (int i = 0; i < starsPerLevel.Length; i++)
        {
            starsPerLevel[i] = PlayerPrefs.GetInt($"Level{(i + 1)}Stars", 0);
        }
    }
}
