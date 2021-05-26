using System.Collections;
using UnityEngine;
using Networking;
using TMPro;
using UnityEngine.Events;
using MLAPI;
using System.Collections.Generic;
using MLAPI.Messaging;
using System.Linq;
using MLAPI.NetworkVariable;

public class GameController : NetworkBehaviour
{
    public enum GameStates
    {
        Initial,                // state of the game at the very start
        Prepare,                // when players need to press "start" button
        Play                    // when the game is going
    }

    public enum PlayerMode
    {
        Singleplayer = 0,
        Multiplayer = 1
    }

    public enum GameMode
    {
        Classic = 0,
        Accuracy = 1
    }

    public NetworkVariable<GameStates> gameState = new NetworkVariable<GameStates>(GameStates.Initial);

    public List<Transform> playersPositions;
    public List<Material> playersMaterials;
    [SerializeField] private CountdownHandler countdown;
    [SerializeField] private ScoreHandler scoreHandler;
    [SerializeField] private GameObject readyButton;
    [SerializeField] TextMeshProUGUI readyButtonText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private NetworkObject ballPrefab;
    [SerializeField] private GameObject serverDisconnectedPanel;
    [SerializeField] private ConnectionManager connectionManager;

    [SerializeField] private TextMeshProUGUI debugText;
    public UnityEvent OnReady;

    /*private PongNetworkDiscovery networkDiscovery;*/
    private BallController ballController;
    private List<NetworkObject> players;
    public PlatformController pitcher; // player who started round
    public GameObject lastFender; // player last reflected ball
    private GameObject gameCanvas;
    private GameObject menuCanvas;

    public bool debugMode;
    public bool testMode;

    public float leftWallPosition;
    public float rightWallPosition;

    public TextMeshProUGUI player1State;
    public TextMeshProUGUI player2State;

    private void Start()
    {
        leftWallPosition = GameObject.Find("LeftSideWall").transform.position.x;
        rightWallPosition = GameObject.Find("RightSideWall").transform.position.x;

        gameCanvas = GameObject.Find("Game");
        menuCanvas = GameObject.Find("Menu");
        players = new List<NetworkObject>();
    }

    public void EnterGame()
    {
        gameCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }

    public void BothPlayersConnected()
    {
        if (!pongManager.IsServer)
        {
            return;
        }
        var player_names = (from player in pongManager.ConnectedClientsList select player.PlayerObject.name).ToArray();
        scoreHandler.InitScore(player_names);
        pitcher = pongManager.ConnectedClientsList[0].PlayerObject.GetComponent<PlatformController>();
        var ball = Instantiate(ballPrefab, pitcher.GetBallStartPosition(), Quaternion.identity);
        ballController = ball.GetComponent<BallController>();
        ball.Spawn();
        gameState.Value = GameStates.Prepare;
    }

    /*public void ServerDisconnected()
    {
        ServerDisconnectedClientRpc();
    }*/

    [ClientRpc]
    public void ServerDisconnectedClientRpc(ClientRpcParams clientRpcParams)
    {
        connectionManager.Leave();
        serverDisconnectedPanel.SetActive(true);
        StopHostServerRpc();
    }

    private void Update()
    {
        if (testMode)
        {
            CheckForDebugTouch();
        }
    }

    public void AddPlayer(NetworkObject player)
    {
        players.Add(player);
    }

    /// <summary>
    /// Check for touches in debug mode. Allows to place ball wherever on a board
    /// </summary>
    private void CheckForDebugTouch()
    {
        if (Input.touchCount != 1)
            return;
        var touch = Input.GetTouch(0);
        if (Camera.main is null)
            return;
        var touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        if (!(touchPosition.x < rightWallPosition) || !(touchPosition.x > leftWallPosition))
            return;
        touchPosition.z = 0f;
        ballController.MoveBall(touchPosition);
    }

    public void OnReadyButtonClicked()
    {
        var player = pongManager.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>();
        readyButtonText.text = player.IsReady.Value ? "Ready" : "Not ready";
        player.TogglePlayerReadyServerRpc();
    }

    public void OnStartButtonClicked()
    {
        StartServerRpc();
    }

    [ServerRpc]
    private void StartServerRpc()
    {
        StartRoundClientRpc();
        /*StartNewRound();*/

        /*make it after coroutine*/
        gameState.Value = GameStates.Play;
        if (!testMode)
        {
            ballController.LaunchBall(pitcher.launchDirection);
        }
    }

    [ClientRpc]
    private void StartRoundClientRpc()
    {
        Debug.Log("StartRoundClientRpc");
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void ReadyToStart(bool everyoneIsReady)
    {
        startButton.SetActive(everyoneIsReady);
    }

    public void StartNewRound()
    {
        if (!debugMode)
        {
            StartCoroutine(StartAfterCountdown());
        }
    }

    public void DestroyBall()
    {
        if (ballController != null)
        {
            Destroy(ballController.gameObject);
        }
    }

    private IEnumerator StartAfterCountdown()
    {
        /*TODO synced countdown*/
        Debug.Log("StartAfterCountdown");
        yield return StartCoroutine(countdown.CountDown());
        /*gameState = GameStates.Play;*/
        if (!testMode)
        {
            ballController.LaunchBall(pitcher.launchDirection);
        }
    }

    public void FinishRound(GameObject winner)
    {
        if (!pongManager.IsServer) return;

        foreach (NetworkObject player in players)
        {
            if (!winner.CompareTag(player.tag))
            {
                pitcher = player.GetComponent<PlatformController>();
            }
            player.GetComponent<PlayerController>().IsReady.Value = false;
        }
        bool hasWinner = scoreHandler.UpdateScore(winner.tag);
        if (hasWinner)
        {
            Debug.Log($"{winner.tag} has won!");
        }

        FinishRoundClientRpc(winner.tag);

        ResetGameObjects();
        gameState.Value = GameStates.Prepare;
    }

    [ClientRpc]
    public void FinishRoundClientRpc(string winnerName)
    {
        readyButtonText.text = "Ready";
        readyButton.SetActive(true);
        startButton.SetActive(false);
    }

    /// <summary>
    /// Called on the server in order to sync between clients
    /// </summary>
    private void ResetGameObjects()
    {
        /*countdown.Reset();*/

        foreach (NetworkObject player in players)
        {
            player.GetComponent<PlatformController>().ResetPlatform();
        }

        ballController.ResetBall(pitcher.GetBallStartPosition());
        gameObject.GetComponent<PowerUpsManager>().clearPowerUps();
        startButton.SetActive(true);
    }

    public void PlatformTouchHandler(GameObject platform)
    {
        /*if (gameState != GameStates.Play)
            return;*/
        lastFender = platform;
        Debug.Log("Triggering power up");
        GetComponent<PowerUpsManager>().TriggerPowerUp();
    }

    [ServerRpc]
    public void StopHostServerRpc()
    {
        pongManager.StopHost();
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