using System.Collections;
/*using Mirror;*/
using UnityEngine;
using Networking;
using TMPro;
using UnityEngine.Events;
using MLAPI;
using MLAPI.Connection;
using System.Collections.Generic;
using MLAPI.Messaging;

public class GameController : NetworkBehaviour
{
    public enum GameStates
    {
        Initial,                // state of the game at the very start
        Prepare,                // when players need to press "start" button
        Ready,                  // when current player waits another player to connect
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

    public List<Transform> playersPositions;
    public List<Color> playersColors;
    [SerializeField] private CountdownHandler countdown;
    [SerializeField] private ScoreHandler scoreHandler;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TextMeshProUGUI startButtonText;

    [SerializeField] private TextMeshProUGUI debugText;
    public UnityEvent OnReady;

    /*private PongNetworkDiscovery networkDiscovery;*/
    private BallController ballController;
    private GameObject[] players;
    public GameStates gameState;
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
        /*networkDiscovery = GameObject.Find("NetworkManager").GetComponent<PongNetworkDiscovery>();*/
        gameState = GameStates.Initial;

        leftWallPosition = GameObject.Find("LeftSideWall").transform.position.x;
        rightWallPosition = GameObject.Find("RightSideWall").transform.position.x;

        gameCanvas = GameObject.Find("Game");
        menuCanvas = GameObject.Find("Menu");
        /*gameCanvas.SetActive(false);*/
    }

    public void EnterGame()
    {
        gameCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }

    public void InitGame()
    {
        Debug.Log("InitGame called");
        /*networkDiscovery.StopDiscovery();*/
        ballController = GameObject.FindWithTag("Ball").GetComponent<BallController>();
        players = GameObject.FindGameObjectsWithTag("Player");
        pitcher = players[0].GetComponent<PlatformController>();
        lastFender = players[0];

        //if ((PlayerMode) PlayerPrefs.GetInt("PlayerMode") == PlayerMode.Multiplayer)
        //{
        //    players[1].AddComponent<PlayerController>();
        //}
        //else
        //{
        //    players[1].AddComponent<AIController>();
        //}

        gameState = GameStates.Initial;
    }

    private void Update()
    {
        //if (gameState == GameStates.Initial)
        //{
        //    ResetGameObjects();
        //    gameState = GameStates.Prepare;
        //}

        if (testMode)
        {
            CheckForDebugTouch();
        }

        //debugText.text = 
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
        NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>().TogglePlayerReadyServerRpc();
    }

    public void ReadyToStart()
    {
        readyButton.SetActive(false);
        startButton.SetActive(true);
    }

    public void StartNewRound()
    {
        if (!debugMode)
        {
            StartCoroutine(StartAfterCountdown());
        }
    }

    private IEnumerator StartAfterCountdown()
    {
        yield return StartCoroutine(countdown.CountDown());
        gameState = GameStates.Play;
        if (!testMode)
        {
            ballController.LaunchBall();
        }
    }

    public void FinishRound(GameObject winner)
    {
        scoreHandler.updateScore(winner.tag);

        foreach (GameObject player in players)
        {
            if (!winner.CompareTag(player.name))
            {
                pitcher = player.GetComponent<PlatformController>();
                break;
            }
        }

        ResetGameObjects();
        gameState = GameStates.Prepare;
    }

    private void ResetGameObjects()
    {
        countdown.Reset();

        foreach (GameObject player in players)
        {
            player.GetComponent<PlatformController>().ResetPlatform();
        }

        ballController.ResetBall(pitcher.GetBallStartPosition());
        gameObject.GetComponent<PowerUpsManager>().clearPowerUps();
        //startButton.SetActive(true);
    }

    public void PlatformTouchHandler(GameObject platform)
    {
        if (gameState != GameStates.Play)
            return;
        lastFender = platform;
        GetComponent<PowerUpsManager>().TriggerPowerUp();
    }

    private PongManager pnm;

    private PongManager networkManager
    {
        get
        {
            if (pnm != null) { return pnm; }
            return pnm = NetworkManager.Singleton as PongManager;
        }
    }
}