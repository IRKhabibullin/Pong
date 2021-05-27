using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using System.Linq;
using UnityEngine;
using static GameController;

public class PlayerController : NetworkBehaviour {
    private PlatformController platform;

    //Rotation controlling values
    private float lastAngle;
    private float startAngle;
    private float angleBetweenTouches;

    public GameController gameController;
    private Camera mainCamera;

    public string _name = "";

    public NetworkVariableVector3 Position = new NetworkVariableVector3();
    public NetworkVariableQuaternion Rotation = new NetworkVariableQuaternion();
    public NetworkVariableBool IsReady = new NetworkVariableBool();
    public NetworkVariableBool IsLeader = new NetworkVariableBool();

    void Start() {
        mainCamera = Camera.main;
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        platform = GetComponent<PlatformController>();
    }

    private void Update()
    {
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
    public void SetColorClientRpc(int playerNumber)
    {
        GetComponent<MeshRenderer>().material = gameController.playersMaterials[playerNumber];
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
        if (gameController.gameState.Value == GameStates.Prepare)
        {
            return;
        }
        var direction = Input.GetAxis("Horizontal");
        if (direction != 0.0 || platform.mSpeed != Vector3.zero)
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
}
