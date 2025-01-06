using Unity.Netcode;
using UnityEngine;

public class ClientConnection : MonoBehaviour
{
    public string LevelName;

    void Update(){
        // if(Input.GetKeyDown(KeyCode.Alpha1) && !NetworkManager.Singleton.IsConnectedClient){
        //     ConnectToLevel(LevelName);
        // }
    }
    public void ConnectToLevel(string levelName)
    {
        var serverInfo = MatchmakingServer.Instance.GetServerForLevel(levelName);

        if (serverInfo != null)
        {
            Debug.Log($"Connecting to {serverInfo.IpAddress}:{serverInfo.Port}");

            // Example: Set connection data and start the client
            NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(levelName);
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogWarning("No available server for the selected level.");
        }
    }
}
