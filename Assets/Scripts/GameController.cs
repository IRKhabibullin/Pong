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
    private const string ReadyText = "Ready";
    private const string NotReadyText = "Not ready";

    [SerializeField] private NetworkObject ballPrefab;

    public PlatformController pitcher; // player who started round
    public GameObject lastFender; // player last reflected ball

    public bool debugMode;
    private IEnumerator countdownEnumerator;

    #region Connection handlers
    public void OnBothPlayersConnected()
    {
        if (!pongManager.IsServer) return;

        var player_names = (from player in pongManager.ConnectedClientsList select player.PlayerObject.tag).ToArray();
        scoreHandler.InitScore(player_names);
        lastFender = pongManager.ConnectedClientsList[0].PlayerObject.gameObject;
        pitcher = lastFender.GetComponent<PlatformController>();

        // create network synced ball and find it on clients
        var ball = Instantiate(ballPrefab, pitcher.GetBallStartPosition(), Quaternion.identity);
        ball.Spawn();
        FindBallClientRpc();

        gameState.Value = GameStates.Prepare;
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

    /// <summary>
    /// Called when client or server disconnects
    /// </summary>
    public void QuitToMenu()
    {
        menuPanel.SetActive(true);
        leaveButton.SetActive(false);
        readyButton.SetActive(false);
        readyButtonText.text = ReadyText;
        pongManager.ConnectedClients[pongManager.LocalClientId].PlayerObject.GetComponent<PlayerController>().IsReady.Value = false;
    }

    public void BeforeStopHost()
    {
        Debug.Log($"Stop host {countdownEnumerator} {countdownEnumerator != null}");
        countdownHandler.StopCountdown();
        if (countdownEnumerator != null)
        {
            StopCoroutine(countdownEnumerator);
        }
        DestroyBall();
        ResetGameObjectsServerRpc();
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
            countdownEnumerator = StartAfterCountdown();
            StartCoroutine(countdownEnumerator);
        }
    }

    private IEnumerator StartAfterCountdown()
    {
        ResetGameObjectsServerRpc();
        StartRoundClientRpc();
        yield return StartCoroutine(countdownHandler.CountDown());
        gameState.Value = GameStates.Play;
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

    public void FinishRound(GameObject winner)
    {
        if (!pongManager.IsServer) return;

        FinishRoundClientRpc();
        ballController.StopBall();
        gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
        gameState.Value = GameStates.Prepare;

        string winner_player = "";
        foreach (var _client in pongManager.ConnectedClientsList)
        {
            NetworkObject player = _client.PlayerObject;
            if (!winner.CompareTag(player.tag))
            {
                pitcher = player.GetComponent<PlatformController>();
            } else
            {
                winner_player = player.GetComponent<PlayerController>().Name.Value;
            }
            player.GetComponent<PlayerController>().IsReady.Value = false;
        }

        bool hasWinner = scoreHandler.UpdateScore(winner.tag);
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
        if (gameState.Value != GameStates.Play) return;
        lastFender = platform;
        GetComponent<PowerUpsManager>().TriggerPowerUp();
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