using System.Collections;
using UnityEngine;
using TMPro;
using MLAPI;
using System.Collections.Generic;
using MLAPI.Messaging;
using System.Linq;
using MLAPI.NetworkVariable;

public class GameController : NetworkBehaviour
{
    #region Variables
    private const string ReadyText = "Ready";
    private const string NotReadyText = "Not ready";

    public enum GameState
    {
        Initial,                // state of the game at the very start
        Prepare,                // when players need to press "start" button
        Play                    // when the game is going
    }

    public enum GameMode
    {
        Classic = 0,
        Accuracy = 1
    }

    public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.Initial);
    public GameMode gameMode;
    public bool playWithBot = false;

    public List<Transform> playersPositions;
    public List<Material> playersMaterials;

    public BallController ballController;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private ScoreHandler scoreHandler;
    [SerializeField] private CountdownHandler countdownHandler;

    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject waitingForOpponentPanel;

    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] TextMeshProUGUI readyButtonText;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject aiPlayerPrefab;

    [SerializeField] private TextMeshProUGUI debugText;

    public PlatformController pitcher; // player who started round
    public GameObject lastTouched; // player last reflected ball

    public PlatformController playerPlatform; // player in game with bots
    public PlatformController botPlatform;

    public bool debugMode;
    private Coroutine countdownCoroutine;
    #endregion

    public void ToggleDebugMode(bool newValue)
    {
        debugMode = newValue;
    }

    public void SetDebugText(string text)
    {
        debugText.text = text;
    }

    #region Connection handlers
    public void LoadGameMode()
    {
        GameObject.Find("GameModeDropdown").GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("GameMode");
    }

    public void PlayWithBots() {
        playWithBot = true;
    }

    public void EnterTheGame()
    {
        menuPanel.SetActive(false);
        leaveButton.SetActive(true);
        if (playWithBot)
        {
            gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
            startButton.SetActive(true);

            lastTouched = Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);
            lastTouched.tag = "Player1";
            playerPlatform = lastTouched.GetComponent<PlatformController>();
            playerPlatform.SetUp(0, playWithBot);
            pitcher = playerPlatform;
            var bot = Instantiate(aiPlayerPrefab, aiPlayerPrefab.transform.position, aiPlayerPrefab.transform.rotation);
            bot.tag = "Player2";
            botPlatform = bot.GetComponent<PlatformController>();
            botPlatform.SetUp(1, playWithBot);

            scoreHandler.InitScore(playWithBot);
            GetComponent<PowerUpsManager>().SetUpPowerUpsTrigger();

            var ball = Instantiate(ballPrefab, pitcher.GetBallStartPosition(), Quaternion.identity);
            ballController = ball.GetComponent<BallController>();

            gameState.Value = GameState.Prepare;
        }
        else
        {
            if (IsServer)
            {
                waitingForOpponentPanel.SetActive(true);
                gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
                gameState.Value = GameState.Initial;
            }
            if (!IsServer)
                ResetReadyState();
        }
    }

    public void OnBothPlayersConnected()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        scoreHandler.InitScore(playWithBot);
        GetComponent<PowerUpsManager>().SetUpPowerUpsTrigger();
        lastTouched = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.gameObject;
        pitcher = lastTouched.GetComponent<PlatformController>();
        waitingForOpponentPanel.SetActive(false);
        ResetReadyState();

        // create network synced ball and find it on clients
        var ball = Instantiate(ballPrefab.GetComponent<NetworkObject>(), pitcher.GetBallStartPosition(), Quaternion.identity);
        ball.Spawn();
        FindBallClientRpc();
        if (gameMode == GameMode.Accuracy)
        {
            ball.GetComponent<BallController>().ChangeMaterialClientRpc(lastTouched.tag);
        }

        gameState.Value = GameState.Prepare;
    }

    public void QuitToMenu()
    {
        menuPanel.SetActive(true);
        leaveButton.SetActive(false);
        readyButton.SetActive(false);
        startButton.SetActive(false);
        countdownHandler.ResetCountdown();
    }

    public void ResetReadyState()
    {
        NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<PlayerController>().IsReady.Value = false;
        readyButtonText.text = ReadyText;
        readyButton.SetActive(true);
    }

    [ClientRpc]
    public void FindBallClientRpc()
    {
        ballController = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
    }

    public void BeforeStopHost()
    {
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);
        waitingForOpponentPanel.SetActive(false);
        startButton.SetActive(false);
        if (ballController != null)
            Destroy(ballController.gameObject);
    }
    #endregion

    #region Round handling
    public void ReadyToStart(bool everyoneIsReady)
    {
        startButton.SetActive(everyoneIsReady);
    }

    /// <summary>
    /// Called on the server
    /// </summary>
    public void StartRound()
    {
        if (debugMode)
            return;
        if (!playWithBot)
            foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
                _client.PlayerObject.GetComponent<PlatformController>().ResetPlatform();
        else {
            playerPlatform.ResetPlatform();
            botPlatform.ResetPlatform();
            startButton.SetActive(false);
        }
        ballController.ResetBall();
        StartCoroutine(StartAfterCountdown());
    }

    private IEnumerator StartAfterCountdown()
    {
        PrepareForRoundClientRpc();
        countdownCoroutine = StartCoroutine(countdownHandler.CountDown());
        yield return countdownCoroutine;

        gameState.Value = GameState.Play;
        if (!debugMode)
        {
            ballController.LaunchBall();
        }
    }

    [ClientRpc]
    private void PrepareForRoundClientRpc()
    {
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void FinishRound()
    {
        FinishRoundClientRpc();
        ballController.StopBall();
        gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
        gameState.Value = GameState.Prepare;
    }

    [ClientRpc]
    public void FinishRoundClientRpc()
    {
        ResetReadyState();
        startButton.SetActive(false);
    }

    public void FinishMatch(string winnerName)
    {
        scoreHandler.ClearScores();
        WinnerNotificationClientRpc(winnerName);
    }

    [ClientRpc]
    public void WinnerNotificationClientRpc(string winner) {
        winnerPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{winner} won!";
        winnerPanel.SetActive(true);
    }
    #endregion

    #region Ingame handlers
    /// <summary>
    /// Handler for accuracy mode only
    /// </summary>
    public void PlatformTouchHandler(GameObject platform)
    {
        if (gameMode != GameMode.Accuracy || gameState.Value != GameState.Play) return;

        lastTouched = platform;
        ballController.ChangeMaterialClientRpc(lastTouched.tag);
        GetComponent<PowerUpsManager>().TriggerPowerUp();
    }

    public void BackWallTouchHandler(string playerTag)
    {
        if (gameMode == GameMode.Classic)
        {
            FinishRound();
            bool hasWinner = scoreHandler.UpdateScore(playerTag);
            foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject player = _client.PlayerObject;
                if (!player.gameObject.CompareTag(playerTag))
                    pitcher = player.GetComponent<PlatformController>();
                else if (hasWinner)
                {
                    FinishMatch(player.GetComponent<PlayerController>().Name.Value);
                    break;
                }
            }
        }
        else if (gameMode == GameMode.Accuracy)
        {
            foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject player = _client.PlayerObject;
                if (player.gameObject.CompareTag(playerTag))
                {
                    lastTouched = player.gameObject;
                    break;
                }
            }
            ballController.ChangeMaterialClientRpc(playerTag);
        }
    }

    public void PowerUpTouchHandler()
    {
        if (gameMode != GameMode.Accuracy) return;

        bool hasWinner = scoreHandler.UpdateScore(lastTouched.tag);
        if (hasWinner)
        {
            FinishRound();
            foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject player = _client.PlayerObject;
                if (player.gameObject.CompareTag(lastTouched.tag))
                {
                    FinishMatch(player.GetComponent<PlayerController>().Name.Value);
                    break;
                }
            }
        }
    }

    public void OnReadyButtonClicked()
    {
        var player = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>();
        readyButtonText.text = player.IsReady.Value ? ReadyText : NotReadyText;
        player.TogglePlayerReadyServerRpc();
    }
    #endregion
}