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
        pongManager.OnServerStarted += HandleServerStarted;
        pongManager.OnClientConnectedCallback += HandleClientConnected;
        pongManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong ClientId)
    {
        if (ClientId == pongManager.LocalClientId)
        {
            menuPanel.SetActive(true);
            leaveButton.SetActive(false);
        }
    }

    private void HandleClientConnected(ulong ClientId)
    {
        if (ClientId == pongManager.LocalClientId)
        {
            menuPanel.SetActive(false);
            leaveButton.SetActive(true);
        }
        if (pongManager.IsServer)
        {
            int i = 0;
            foreach (NetworkClient client in pongManager.ConnectedClientsList)
            {
                if (client.ClientId == ClientId)
                {
                    var player = client.PlayerObject.GetComponent<PlayerController>();
                    player.Position.Value = gameController.playersPositions[i].position;
                    player.Rotation.Value = gameController.playersPositions[i].rotation;
                    player.SetColorClientRpc(gameController.playersColors[i]);
                    if (pongManager.ConnectedClients.Count == 1)
                    {
                        player.IsLeader.Value = true;
                        Debug.Log($"Qwe {player.IsLeader.Value} {player.OwnerClientId} {pongManager.ConnectedClients.Count}");
                    }
                    break;
                }
                i++;
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
