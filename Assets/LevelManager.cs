using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

        [System.Serializable]
    public struct LevelInfo
    {
        public string levelName;
        public Transform spawnPoint; // Spawn point for this level
    }

    public LevelInfo[] levels;
    public static LevelManager Instance;

    [Header("Cameras")]
    public Camera introCamera; // Intro camera for level showcase
    public Camera gameplayCamera; // Main gameplay camera

    [Header("Camera Path")]
    public Transform cameraPathParent; // Parent of the camera path waypoints

    private List<Transform> pathPoints;
    private bool isCutsceneActive = false; // Tracks if the cutscene is active
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Gather all waypoints from the camera path
        pathPoints = new List<Transform>();
        foreach (Transform child in cameraPathParent)
        {
            pathPoints.Add(child);
        }
    }

    /// <summary>
    /// Spawns the player's marble at the starting point.
    /// </summary>
    /// <param name="clientId">The client ID for ownership of the spawned marble.</param>
    /// <param name="marblePrefab">The prefab of the marble to spawn.</param>
    public void SpawnPlayerMarble(ulong clientId, GameObject marblePrefab)
    {
        if (levels[0].spawnPoint == null)
        {
            Debug.LogError("Starting point is not assigned in LevelManager!");
            return;
        }

        // Instantiate the marble at the starting point
        GameObject marble = Instantiate(marblePrefab, levels[0].spawnPoint.position, Quaternion.identity);

        // Get the NetworkObject and spawn it with ownership
        NetworkObject networkObject = marble.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
        }
        else
        {
            Debug.LogError("The marble prefab does not have a NetworkObject component.");
        }
    }

    /// <summary>
    /// Teleports the player to the starting point of the level.
    /// </summary>
    /// <param name="player">The player's GameObject.</param>
    public void TeleportToStartingPoint(int levelIndex, GameObject player)
    {
        if (levels[levelIndex].spawnPoint == null)
        {
            Debug.LogWarning("Starting point is not set in LevelManager!");
            return;
        }

        Debug.Log($"Teleporting player {player.name} to starting point at {levels[levelIndex].spawnPoint.position}...");

        // Reset the player's position
        player.transform.position = levels[levelIndex].spawnPoint.position;

        // Reset momentum
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Temporarily set kinematic
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false; // Restore to non-kinematic state
        }
    }

    /// <summary>
    /// Starts the level intro cutscene.
    /// </summary>
    public void StartLevelIntro()
    {
        StartCoroutine(ShowLevelIntro());
    }
/// <summary>
/// Teleports the player to the specified level's spawn point.
/// </summary>
/// <param name="player">The player's GameObject.</param>
/// <param name="levelIndex">The index of the level to teleport to.</param>
public void TeleportPlayerToLevel(GameObject player, int levelIndex)
{
    if (levelIndex < 0 || levelIndex >= pathPoints.Count)
    {
        Debug.LogError("Invalid level index!");
        return;
    }

    Transform spawnPoint = levels[levelIndex].spawnPoint;

    if (spawnPoint != null)
    {
        // Reset the player's position to the spawn point
        Debug.Log($"Teleporting player {player.name} to level {levelIndex} at position {spawnPoint.position}");
        player.transform.position = spawnPoint.position;

        // Reset momentum
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Temporarily set kinematic
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false; // Restore to non-kinematic state
        }

            MarbleMovement marble = FindObjectOfType<MarbleMovement>();
            if (marble != null)
            {
                marble.EnableMovement();
            }
    }
    else
    {
        Debug.LogError($"Spawn point for level {levelIndex} is not assigned!");
    }
}

    private IEnumerator ShowLevelIntro()
    {
        if (introCamera != null && gameplayCamera != null)
        {
            Debug.Log("Starting Level Intro");

            // Enable Intro Camera
            introCamera.gameObject.SetActive(true);
            gameplayCamera.gameObject.SetActive(false);

            isCutsceneActive = true;

            float panDuration = 7.0f;
            float elapsedTime = 0f;

            while (elapsedTime < panDuration)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Cutscene skipped");
                    break; // Exit the loop and skip the cutscene
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure camera pan ends immediately if skipped
            CameraPanManager.Instance.ShowcaseLevel(introCamera.transform, levels[0].spawnPoint, pathPoints, 0);

            // Switch to Gameplay Camera
            introCamera.gameObject.SetActive(false);
            gameplayCamera.gameObject.SetActive(true);

            // Enable player movement
            MarbleMovement player = FindObjectOfType<MarbleMovement>();
            if (player != null)
            {
                player.EnableMovement();
            }

            isCutsceneActive = false;
        }
        else
        {
            Debug.LogWarning("Intro or Gameplay camera is not assigned in LevelManager.");
        }
    }

    /// <summary>
    /// Handles level completion logic for the local player.
    /// </summary>
    public void CompleteLevel(GameObject player)
    {
        if (levelComplete) return;

        levelComplete = true;

        // Notify the client to handle their specific end-level logic
        HandleClientLevelComplete(player);
    }

    /// <summary>
    /// Handles level completion on the client side.
    /// </summary>
    /// <param name="player">The local player's GameObject.</param>
    public void HandleClientLevelComplete(GameObject player)
    {
        Debug.Log("Client handling level completion...");

        // Stop player movement
        MarbleMovement marbleMovement = player.GetComponent<MarbleMovement>();
        if (marbleMovement != null)
        {
            marbleMovement.StopMovement();
        }

        // Perform a cinematic pan around the player
        if (CameraPanManager.Instance != null)
        {
            CameraPanManager.Instance.PanAroundTarget(Camera.main.transform, player.transform);
        }

        // Display level completion UI (next level or return to hub)
        if (Userinterface.Instance != null)
        {
            Userinterface.Instance.OpenUI(OriginLabs.MenuType.LevelSelect);
        }
    }
}
