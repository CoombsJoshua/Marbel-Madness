using System.Collections;
using OriginLabs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("References")]
    public Userinterface userInterface; // Reference to the UI system

    private bool levelComplete = false;

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

    public void EnterLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex}", LoadSceneMode.Single);
        StartCoroutine(HandleLevelStart());
    }

    private IEnumerator HandleLevelStart()
    {
        yield return new WaitForSeconds(1); // Allow level to load

        // Delegate intro to the LevelManager
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevelIntro();
        }
        else
        {
            Debug.LogWarning("LevelManager not found in the scene.");
        }
    }

    // public void ResetPlayer(GameObject player)
    // {
    //     if (LevelManager.Instance == null) return;

    //     player.transform.position = LevelManager.Instance.startingPoint.position;
    //     Rigidbody rb = player.GetComponent<Rigidbody>();
    //     if (rb != null)
    //     {
    //         rb.linearVelocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //     }
    // }

    public void CompleteLevel(GameObject player)
    {
        if (levelComplete) return;
        levelComplete = true;

        MarbleMovement marbleMovement = player.GetComponent<MarbleMovement>();
        if (marbleMovement != null)
        {
            marbleMovement.StopMovement();
        }

        // Trigger camera pan
        CameraPanManager.Instance.PanAroundTarget(Camera.main.transform, player.transform);

        // Reward XP and show completion popup
        StartCoroutine(HandleLevelCompletion());
    }

    private IEnumerator HandleLevelCompletion()
    {
        // Wait for the camera pan to complete
        yield return new WaitForSeconds(5.0f); // Adjust based on pan duration

        RewardXP(100); // Example XP reward
        userInterface.OpenUI(MenuType.LevelSelect);
    }

    private void RewardXP(int xp)
    {
        Debug.Log($"Rewarded {xp} XP!");
        // Implement XP logic here (e.g., update player stats)
    }
}
