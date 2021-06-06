using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using UnityEngine;

public class PlayerController : NetworkBehaviour {

    public GameController gameController;

    public string _name = "";

    public NetworkVariableVector3 Position = new NetworkVariableVector3();
    public NetworkVariableQuaternion Rotation = new NetworkVariableQuaternion();
    public NetworkVariableBool IsReady = new NetworkVariableBool();
    public NetworkVariableBool IsLeader = new NetworkVariableBool();

    void Start() {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    [ClientRpc]
    public void SetPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    [ClientRpc]
    public void SetColorClientRpc(int playerNumber)
    {
        GetComponent<MeshRenderer>().material = gameController.playersMaterials[playerNumber];
    }

    void OnDrawGizmos() {
        if (Input.touchCount == 2) {
            Vector3 firstTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector3 secondTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);
            firstTouch.z = 0;
            secondTouch.z = 0;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firstTouch, secondTouch);
        }
    }

    [ServerRpc]
    public void TogglePlayerReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
        bool everyoneIsReady = true;
        foreach (var client in pongManager.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerController>().IsReady.Value && client.ClientId != pongManager.LocalClientId)
            {
                everyoneIsReady = false;
                break;
            }
        }
        if (pongManager.ConnectedClientsList.Count == 2) {
            UpdateReadyStatusClientRpc(everyoneIsReady);
        }
    }

    [ClientRpc]
    public void UpdateReadyStatusClientRpc(bool everyoneIsReady)
    {
        if (pongManager.IsHost)
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
