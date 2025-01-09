using UnityEngine;
using Unity.Netcode;

public class MarbleSpawner : MonoBehaviour
{
    public GameObject marblePrefab; // Assign the marble prefab in the Inspector

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (LevelManager.Instance != null)
        {
            // Spawn the marble for the connected client
            LevelManager.Instance.SpawnPlayerMarble(clientId, marblePrefab);
        }
        else
        {
            Debug.LogError("LevelManager is not assigned or available.");
        }
    }
}
