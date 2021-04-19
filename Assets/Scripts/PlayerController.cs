using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using System;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    private PlatformController platform;

    //Rotation controlling values
    private float lastAngle;
    private float startAngle;
    private float angleBetweenTouches;

    private GameController gameController;
    private Camera mainCamera;

    public string _name = "";

    public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableQuaternion Rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableBool IsReady = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableBool IsLeader = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    /*private bool isLeader
    {
        get
        {
            return networkManager.players[0].connectionToClient == connectionToClient;
        }
    }*/

    void Start() {
        mainCamera = Camera.main;
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        platform = GetComponent<PlatformController>();
    }

    private void Update()
    {
        /*transform.position = Position.Value;
        transform.rotation = Rotation.Value;*/
        if (IsLocalPlayer)
        {
            CheckForKeyboard();
        }
    }

    [ClientRpc]
    public void SetPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    [ClientRpc]
    public void SetColorClientRpc(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    void CheckForTouch() {
        if (Input.touchCount == 1) {
            // we're moving
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            if (Mathf.Abs(touchPosition.x) < gameController.rightWallPosition) {
                return;
            }
            if (touchPosition.x < gameController.leftWallPosition) {
                platform.MoveServerRpc(-1f);
            }
            if (touchPosition.x > gameController.rightWallPosition) {
                platform.MoveServerRpc(1f);
            }
        } else if (Input.touchCount == 2) {
            // we're rotating
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);
            Vector2 firstTouchPosition = mainCamera.ScreenToWorldPoint(firstTouch.position);
            Vector2 secondTouchPosition = mainCamera.ScreenToWorldPoint(secondTouch.position);
            if (Mathf.Abs(firstTouchPosition.x) < gameController.rightWallPosition || Mathf.Abs(secondTouchPosition.x) < gameController.rightWallPosition) {
                return;
            }
            angleBetweenTouches = CalcCurrentAngle(firstTouchPosition, secondTouchPosition);
            // todo maybe chech that player could make both touches on one side
            if (firstTouch.phase.Equals(TouchPhase.Began) || secondTouch.phase.Equals(TouchPhase.Began)) {
                lastAngle = platform.GetCurrentAngle();
                startAngle = angleBetweenTouches;
            } else if (firstTouch.phase.Equals(TouchPhase.Moved) || secondTouch.phase.Equals(TouchPhase.Moved)) {
                platform.Rotate(lastAngle + angleBetweenTouches - startAngle);
            }
        }
    }

    void CheckForKeyboard()
    {
        var direction = Input.GetAxis("Horizontal");
        if (direction != 0.0)
        {
            platform.MoveServerRpc(direction);
        }
    }

    float CalcCurrentAngle(Vector2 firstPoint, Vector2 secondPoint) {
        Vector2 direction = firstPoint.x < secondPoint.x ? secondPoint - firstPoint : firstPoint - secondPoint;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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

    private void HandleNewMessage(string message)
    {
        Debug.Log($"Opponent says: {message}");
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => CheckReady();

    private void CheckReady()
    {
        /*if (!isLeader)
        {
            return;
        }*/
        /*foreach (var player in networkManager.players)
        {
            if (!player.isReady)
            {
                return;
            }
        }*/

        gameController.ReadyToStart();
    }

    /*[Command]
    public void CmdStartGame()
    {
        if (!isLeader) {
            return;
        }
        // start game
    }*/

    private PongManager pnm;

    private PongManager pongManager
    {
        get
        {
            if (pnm != null) { return pnm; }
            return pnm = NetworkManager.Singleton as PongManager;
        }
    }

    [ServerRpc]
    public void TogglePlayerReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
        UpdateReadyStatusClientRpc(new bool[] {
            pongManager.ConnectedClientsList[0].PlayerObject.GetComponent<PlayerController>().IsReady.Value,
            pongManager.ConnectedClientsList[1].PlayerObject.GetComponent<PlayerController>().IsReady.Value
        });
    }

    [ClientRpc]
    public void UpdateReadyStatusClientRpc(bool[] readyStatuses)
    {
        if (readyStatuses.Length > 0)
        {
            gameController.player1State.text = readyStatuses[0].ToString();
            if (readyStatuses.Length > 1)
            {
                gameController.player2State.text = readyStatuses[1].ToString();
            }
        }
        if (IsLeader.Value)
        {
            bool everyoneIsReady = true;
            foreach (var client in pongManager.ConnectedClientsList)
            {
                if (!client.PlayerObject.GetComponent<PlayerController>().IsReady.Value && client.ClientId != pongManager.LocalClientId)
                {
                    everyoneIsReady = false;
                    break;
                }
            }
            if (everyoneIsReady)
            {
                gameController.ReadyToStart();
            }
        }
    }
}
