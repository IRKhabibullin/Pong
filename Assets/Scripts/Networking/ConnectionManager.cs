using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using Networking;
using MLAPI.Messaging;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameController gameController;

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
    /// This callback is invoked on a client when the server shuts down or a client disconnects. It is only run on the server and the local client that disconnects.
    /// </summary>
    private void HandleClientDisconnect(ulong ClientId)
    {
        if (ClientId == pongManager.LocalClientId)
        {
            menuPanel.SetActive(true);
            leaveButton.SetActive(false);
            readyButton.SetActive(false);
            pongManager.ConnectedClients[pongManager.LocalClientId].PlayerObject.GetComponent<PlayerController>().IsReady.Value = false;
        }
        if (pongManager.IsHost)
        {
            gameController.DestroyBall();
        }
    }

    /// <summary>
    /// runs on the server and on the local client that connects
    /// </summary>
    private void HandleClientConnected(ulong ClientId)
    {
        if (ClientId == pongManager.LocalClientId)
        {
            menuPanel.SetActive(false);
            leaveButton.SetActive(true);
            readyButton.SetActive(true);
        }
        if (pongManager.IsServer)
        {
            int i = 0;
            foreach (NetworkClient client in pongManager.ConnectedClientsList)
            {
                if (client.ClientId == ClientId)
                {
                    client.PlayerObject.tag = $"Player{i + 1}";
                    client.PlayerObject.name = $"Player{i + 1}";
                    var platform = client.PlayerObject.GetComponent<PlatformController>();
                    platform.launchDirection = i == 0 ? 1 : -1;
                    platform.mPosition.Value = gameController.playersPositions[i].position;
                    platform.SetColorClientRpc(i);
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
                gameController.BothPlayersConnected();
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

    public void Host()
    {
        pongManager.StartHost();
    }

    public void Join()
    {
        pongManager.StartClient();
    }

    public void Leave()
    {
        if (pongManager.IsHost)
        {
            if (pongManager.ConnectedClientsList.Count < 2)
            {
                pongManager.StopHost();
            }
            // if server is leaving, notify another client about it
            else if (pongManager.IsServer)
            {
                foreach (var playerClientId in pongManager.ConnectedClients.Keys)
                {
                    if (playerClientId != pongManager.LocalClientId)
                    {
                        ClientRpcParams clientRpcParams = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { playerClientId }
                            }
                        };
                        gameController.ServerDisconnectedClientRpc(clientRpcParams);
                    }
                }
            }
        }
        else if (pongManager.IsClient)
        {
            gameController.ResetGameObjectsServerRpc();
            pongManager.StopClient();
        }
        menuPanel.SetActive(true);
        leaveButton.SetActive(false);
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
