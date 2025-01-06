using Unity.Netcode;
using UnityEngine;

public class LevelServer : MonoBehaviour
{
    public int maxPlayers = 100;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        // Start the server
        NetworkManager.Singleton.StartServer();
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"Player {clientId} connected.");

        // Check max players
        if (NetworkManager.Singleton.ConnectedClients.Count > maxPlayers)
        {
            Debug.LogWarning("Server full! Disconnecting new player.");
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log($"Player {clientId} disconnected.");
    }
}
