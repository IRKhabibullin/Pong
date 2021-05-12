using UnityEngine;
using MLAPI;
using System;
using MLAPI.Connection;
using MLAPI.Messaging;
using Networking;

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
                    var player = client.PlayerObject.GetComponent<PlayerController>();
                    player.SetPositionClientRpc(gameController.playersPositions[i].position, gameController.playersPositions[i].rotation);
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
                Debug.Log("Two players");
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
            pongManager.StopHost();
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
