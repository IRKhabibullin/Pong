using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using Networking;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject serverDisconnectedPanel;
    [SerializeField] private PongNetworkDiscovery discovery;

    public string playerName = "";

    private void Start()
    {
        pongManager.OnServerStarted += HandleServerStarted;
        pongManager.OnClientConnectedCallback += HandleClientConnected;
        pongManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if (pongManager == null) { return; }
        pongManager.OnServerStarted -= HandleServerStarted;
        pongManager.OnClientConnectedCallback -= HandleClientConnected;
        pongManager.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    /// <summary>
    /// This callback is invoked on a client when the server shuts down and invoked on a server when other client disconnects.
    /// </summary>
    private void HandleClientDisconnect(ulong ClientId)
    {
        // Case when server disconnected and we are client
        if (ClientId == pongManager.LocalClientId)
        {
            serverDisconnectedPanel.SetActive(true);
            gameController.QuitToMenu();
        }
        // Case when other client disconnected and we are host
        if (pongManager.IsHost)
        {
            gameController.BeforeStopHost();
        }
    }

    /// <summary>
    /// runs on the server and on the local client that connects
    /// </summary>
    private void HandleClientConnected(ulong ClientId)
    {
        if (ClientId == pongManager.LocalClientId)
        {
            gameController.EnterTheGame();
        }
        if (pongManager.IsServer)
        {
            int i = 0;
            foreach (NetworkClient client in pongManager.ConnectedClientsList)
            {
                if (client.ClientId == ClientId)
                {
                    client.PlayerObject.tag = $"Player{i + 1}";
                    client.PlayerObject.GetComponent<PlatformController>().SetUp(i);
                    if (pongManager.ConnectedClients.Count == 1)
                    {
                        client.PlayerObject.GetComponent<PlayerController>().IsLeader.Value = true;
                    }
                    break;
                }
                i++;
            }
            if (pongManager.ConnectedClientsList.Count == 2)
            {
                discovery.StopDiscovery();
                gameController.OnBothPlayersConnected();
            }
        }
    }

    /// <summary>
    /// Calling here OnClientConnectedCallback because host is server and client at the same time
    /// </summary>
    private void HandleServerStarted()
    {
        if (pongManager.IsHost)
        {
            HandleClientConnected(pongManager.LocalClientId);
        }
    }

    public void Host(string playerName)
    {
        this.playerName = playerName;
        pongManager.StartHost();
    }

    public void Find(string playerName)
    {
        this.playerName = playerName;
        discovery.StartDiscovery();
    }

    public void Join(string hostAddress)
    {
        discovery.StopDiscovery();
        discovery.transport.ConnectAddress = hostAddress;
        pongManager.StartClient();
    }

    public void Leave()
    {
        gameController.QuitToMenu();

        if (pongManager.IsServer)
        {
            // if server is leaving, disconnect another player
            foreach (var playerClientId in pongManager.ConnectedClients.Keys)
            {
                if (playerClientId != pongManager.LocalClientId)
                {
                    pongManager.DisconnectClient(playerClientId);
                    break;
                }
            }
            gameController.BeforeStopHost();
            pongManager.StopHost();
        }
        else
        {
            pongManager.StopClient();
        }
    }

    private PongManager pnm;

    private PongManager pongManager
    {
        get
        {
            if (pnm != null) { return pnm; }
            return pnm = NetworkManager.Singleton as PongManager;
        }
    }
}
