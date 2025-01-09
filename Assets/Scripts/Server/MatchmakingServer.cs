using System.Collections.Generic;
using UnityEngine;

public class MatchmakingServer : MonoBehaviour
{
    public static MatchmakingServer Instance;

    private Dictionary<string, LevelServerInfo> activeServers = new Dictionary<string, LevelServerInfo>();

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
    }

    void Start(){
        MatchmakingServer.Instance.RegisterServer("Level01", "104.207.129.116", 7777, 0, 100);
                MatchmakingServer.Instance.RegisterServer("Level02", "127.0.0.1", 7778, 0, 100);
    }

    public void RegisterServer(string levelName, string ipAddress, int port, int currentPlayers, int maxPlayers)
    {
        activeServers[levelName] = new LevelServerInfo
        {
            IpAddress = ipAddress,
            Port = port,
            CurrentPlayers = currentPlayers,
            MaxPlayers = maxPlayers
        };

        Debug.Log($"Server for level {levelName} registered at {ipAddress}:{port}");
    }

    public LevelServerInfo GetServerForLevel(string levelName)
    {
        if (activeServers.TryGetValue(levelName, out var serverInfo))
        {
            if (serverInfo.CurrentPlayers < serverInfo.MaxPlayers)
            {
                return serverInfo;
            }
        }

        return null; // No available server
    }
}

[System.Serializable]
public class LevelServerInfo
{
    public string IpAddress;
    public int Port;
    public int CurrentPlayers;
    public int MaxPlayers;
}
