using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    private const string ProgressKeyPrefix = "Level_"; // Prefix for level completion keys

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

    /// <summary>
    /// Marks a level as completed and unlocks the next level if applicable.
    /// </summary>
    /// <param name="currentLevel">The current level index.</param>
    public void CompleteLevel(int currentLevel)
    {
        if (!IsLevelCompleted(currentLevel))
        {
            // Mark the current level as completed
            PlayerPrefs.SetInt(ProgressKeyPrefix + currentLevel, 1);

            // Unlock the next level
            PlayerPrefs.SetInt(ProgressKeyPrefix + (currentLevel + 1), 1);
            PlayerPrefs.Save();

            Debug.Log($"Level {currentLevel} completed. Level {currentLevel + 1} unlocked.");
        }
    }

    /// <summary>
    /// Checks if a level is completed.
    /// </summary>
    /// <param name="levelIndex">The level index to check.</param>
    /// <returns>True if the level is completed, false otherwise.</returns>
    public bool IsLevelCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt(ProgressKeyPrefix + levelIndex, 0) == 1;
    }

    /// <summary>
    /// Checks if a level is unlocked.
    /// </summary>
    /// <param name="levelIndex">The level index to check.</param>
    /// <returns>True if the level is unlocked, false otherwise.</returns>
    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex == 1 || IsLevelCompleted(levelIndex - 1);
    }

    /// <summary>
    /// Resets all level progress (for debugging/testing).
    /// </summary>
    public void ResetProgress()
    {
        for (int i = 1; i <= 100; i++) // Adjust range as per the number of levels
        {
            PlayerPrefs.DeleteKey(ProgressKeyPrefix + i);
        }
        PlayerPrefs.Save();

        Debug.Log("All level progress reset.");
    }
}
