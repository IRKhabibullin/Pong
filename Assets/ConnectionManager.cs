using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using Networking;
using System.Collections;
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
                    var player = client.PlayerObject.GetComponent<PlayerController>();
                    player.SetColorClientRpc(i);
                    gameController.AddPlayer(client.PlayerObject);
                    if (pongManager.ConnectedClients.Count == 1)
                    {
                        player.IsLeader.Value = true;
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
