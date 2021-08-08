using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    private GameController gameController;

    public NetworkVariableString Name = new NetworkVariableString(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly });
    public NetworkVariableBool IsReady = new NetworkVariableBool();
    public NetworkVariableBool IsLeader = new NetworkVariableBool();

    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        if (IsOwner)
        {
            Name.Value = PlayerPrefs.GetString("PlayerName");
        }
    }

    [ServerRpc]
    public void TogglePlayerReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
        bool everyoneIsReady = true;
        // if everyone except host is ready, then host can start a round
        foreach (var client in pongManager.ConnectedClientsList)
        {
            if (client.ClientId != pongManager.LocalClientId && !client.PlayerObject.GetComponent<PlayerController>().IsReady.Value)
            {
                everyoneIsReady = false;
                break;
            }
        }
        if (pongManager.ConnectedClientsList.Count == 2)
        {
            gameController.ReadyToStart(everyoneIsReady);
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
