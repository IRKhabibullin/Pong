using System.Collections;
using UnityEngine;
using Networking;
using TMPro;
using MLAPI;
using System.Collections.Generic;
using MLAPI.Messaging;
using System.Linq;
using MLAPI.NetworkVariable;

public class GameController : NetworkBehaviour
{
    private const string ReadyText = "Ready";
    private const string NotReadyText = "Not ready";

    public enum GameState
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

    public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.Initial);
    public GameMode gameMode;

    public List<Transform> playersPositions;
    public List<Material> playersMaterials;

    public BallController ballController;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private ScoreHandler scoreHandler;
    [SerializeField] private CountdownHandler countdownHandler;

    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject menuPanel;

    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] TextMeshProUGUI readyButtonText;

    [SerializeField] private NetworkObject ballPrefab;

    public PlatformController pitcher; // player who started round
    public GameObject lastFender; // player last reflected ball

    public bool debugMode;
    private Coroutine countdownCoroutine;

    #region Connection handlers
    public void OnBothPlayersConnected()
    {
        if (!pongManager.IsServer) return;

        var player_names = (from player in pongManager.ConnectedClientsList select player.PlayerObject.tag).ToArray();
        scoreHandler.InitScore(player_names);
        GetComponent<PowerUpsManager>().SetUpPowerUps();
        lastFender = pongManager.ConnectedClientsList[0].PlayerObject.gameObject;
        pitcher = lastFender.GetComponent<PlatformController>();
        readyButton.SetActive(true);
        gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");

        // create network synced ball and find it on clients
        var ball = Instantiate(ballPrefab, pitcher.GetBallStartPosition(), Quaternion.identity);
        ball.Spawn();
        FindBallClientRpc();
        if (gameMode == GameMode.Accuracy)
        {
            ball.GetComponent<BallController>().ChangeMaterialClientRpc(lastFender.tag);
        }

        gameState.Value = GameState.Prepare;
    }

    public void ToggleDebugMode(bool newValue)
    {
        debugMode = newValue;
    }

    [ClientRpc]
    public void FindBallClientRpc()
    {
        ballController = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
    }

    public void EnterTheGame()
    {
        menuPanel.SetActive(false);
        leaveButton.SetActive(true);
        readyButton.SetActive(true);
    }

    public void QuitToMenu()
    {
        menuPanel.SetActive(true);
        leaveButton.SetActive(false);
        readyButton.SetActive(false);
        readyButtonText.text = ReadyText;
        countdownHandler.ResetCountdown();
        pongManager.ConnectedClients[pongManager.LocalClientId].PlayerObject.GetComponent<PlayerController>().IsReady.Value = false;
    }

    public void BeforeStopHost()
    {
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);
        countdownHandler.ResetCountdown();
        DestroyBall();
    }

    public void DestroyBall()
    {
        if (ballController != null)
        {
            Destroy(ballController.gameObject);
        }
    }
    #endregion

    #region Round handling
    public void ReadyToStart(bool everyoneIsReady)
    {
        startButton.SetActive(everyoneIsReady);
    }

    [ServerRpc]
    public void StartRoundServerRpc()
    {
        if (!debugMode)
        {
            StartCoroutine(StartAfterCountdown());
        }
    }

    private IEnumerator StartAfterCountdown()
    {
        ResetGameObjectsServerRpc();
        StartRoundClientRpc();
        countdownCoroutine = StartCoroutine(countdownHandler.CountDown());
        yield return countdownCoroutine;
        gameState.Value = GameState.Play;
        if (!debugMode)
        {
            ballController.LaunchBall();
        }
    }

    /// <summary>
    /// Called on the server in order to sync between clients
    /// </summary>
    [ServerRpc]
    public void ResetGameObjectsServerRpc()
    {
        foreach (var _client in pongManager.ConnectedClientsList)
        {
            _client.PlayerObject.GetComponent<PlatformController>().ResetPlatform();
        }

        ballController.ResetBall();
        readyButton.SetActive(true);
    }

    [ClientRpc]
    private void StartRoundClientRpc()
    {
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void FinishRound(string winnerTag)
    {
        if (!pongManager.IsServer) return;

        FinishRoundClientRpc();
        ballController.StopBall();
        gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
        gameState.Value = GameState.Prepare;

        string winner_player = "";
        foreach (var _client in pongManager.ConnectedClientsList)
        {
            NetworkObject player = _client.PlayerObject;
            if (!player.gameObject.CompareTag(winnerTag))
            {
                pitcher = player.GetComponent<PlatformController>();
            } else
            {
                winner_player = player.GetComponent<PlayerController>().Name.Value;
            }
            player.GetComponent<PlayerController>().IsReady.Value = false;
        }

        bool hasWinner = scoreHandler.UpdateScore(winnerTag);
        if (hasWinner)
        {
            scoreHandler.ClearScores();
            WinnerNotificationClientRpc(winner_player);
        }
    }

    [ClientRpc]
    public void FinishRoundClientRpc()
    {
        readyButtonText.text = ReadyText;
        readyButton.SetActive(true);
        startButton.SetActive(false);
    }

    [ClientRpc]
    public void WinnerNotificationClientRpc(string winner) {
        winnerPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{winner} won!";
        winnerPanel.SetActive(true);
    }
    #endregion

    #region Ingame handlers
    public void PlatformTouchHandler(GameObject platform)
    {
        if (gameState.Value != GameState.Play) return;
        lastFender = platform;
        ballController.ChangeMaterialClientRpc(lastFender.tag);
        GetComponent<PowerUpsManager>().TriggerPowerUp();
    }

    public void BackWallTouchHandler(GameObject backWall)
    {
        if (gameMode == GameMode.Classic)
        {
            FinishRound(backWall.tag);
        }
        else if (gameMode == GameMode.Accuracy)
        {
            foreach (var _client in pongManager.ConnectedClientsList)
            {
                NetworkObject player = _client.PlayerObject;
                if (!player.gameObject.CompareTag(backWall.tag))
                {
                    lastFender = player.gameObject;
                    break;
                }
            }
            ballController.ChangeMaterialClientRpc(backWall.tag);
        }
    }

    public void OnReadyButtonClicked()
    {
        var player = pongManager.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>();
        readyButtonText.text = player.IsReady.Value ? ReadyText : NotReadyText;
        player.TogglePlayerReadyServerRpc();
    }
    #endregion

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