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
    [SerializeField] private CountdownHandler countdown;
    [SerializeField] private ScoreHandler scoreHandler;
    [SerializeField] private GameObject readyButton;
    [SerializeField] TextMeshProUGUI readyButtonText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private NetworkObject ballPrefab;
    [SerializeField] private GameObject serverDisconnectedPanel;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private ConnectionManager connectionManager;

    [SerializeField] private TextMeshProUGUI debugText;

    /*private PongNetworkDiscovery networkDiscovery;*/
    public BallController ballController;
    public PlatformController pitcher; // player who started round
    public GameObject lastFender; // player last reflected ball

    public bool debugMode;

    public void BothPlayersConnected()
    {
        if (!pongManager.IsServer)
        {
            return;
        }
        var player_names = (from player in pongManager.ConnectedClientsList select player.PlayerObject.name).ToArray();
        scoreHandler.InitScore(player_names);
        pitcher = pongManager.ConnectedClientsList[0].PlayerObject.GetComponent<PlatformController>();
        lastFender = pongManager.ConnectedClientsList[0].PlayerObject.gameObject;
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

    public void DestroyBall()
    {
        if (ballController != null)
        {
            Destroy(ballController.gameObject);
        }
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
        yield return StartCoroutine(countdown.CountDown());
        gameState.Value = GameStates.Play;
        if (!debugMode)
        {
            ballController.LaunchBall();
        }
    }

    [ClientRpc]
    private void StartRoundClientRpc()
    {
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void ReadyToStart(bool everyoneIsReady)
    {
        startButton.SetActive(everyoneIsReady);
    }

    public void FinishRound(GameObject winner)
    {
        if (!pongManager.IsServer) return;

        FinishRoundClientRpc();
        ballController.StopBall();
        gameState.Value = GameStates.Prepare;

        bool hasWinner = scoreHandler.UpdateScore(winner.tag);
        if (hasWinner)
        {
            scoreHandler.ClearScores();
            WinnerNotificationClientRpc(winner.tag);
        }

        foreach (var _client in pongManager.ConnectedClientsList)
        {
            NetworkObject player = _client.PlayerObject;
            if (!winner.CompareTag(player.tag))
            {
                pitcher = player.GetComponent<PlatformController>();
            }
            player.GetComponent<PlayerController>().IsReady.Value = false;
        }
    }

    [ClientRpc]
    public void WinnerNotificationClientRpc(string winner) {
        winnerPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{winner} won!";
        winnerPanel.SetActive(true);
    }

    [ClientRpc]
    public void FinishRoundClientRpc()
    {
        readyButtonText.text = "Ready";
        readyButton.SetActive(true);
        startButton.SetActive(false);
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
        gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
        readyButton.SetActive(true);
    }

    [ClientRpc]
    public void ServerDisconnectedClientRpc(ClientRpcParams clientRpcParams)
    {
        connectionManager.Leave();
        serverDisconnectedPanel.SetActive(true);
        StopHostServerRpc();
    }

    [ServerRpc]
    public void StopHostServerRpc()
    {
        ResetGameObjectsServerRpc();
        pongManager.StopHost();
    }

    #region handlers
    public void PlatformTouchHandler(GameObject platform)
    {
        /*if (gameState != GameStates.Play)
            return;*/
        lastFender = platform;
        GetComponent<PowerUpsManager>().TriggerPowerUp();
    }

    public void OnReadyButtonClicked()
    {
        var player = pongManager.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>();
        readyButtonText.text = player.IsReady.Value ? "Ready" : "Not ready";
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