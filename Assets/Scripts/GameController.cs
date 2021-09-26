using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Singleplayer;
using Multiplayer;
using MLAPI;

public class GameController : MonoBehaviour
{
    #region Variables
    public const string ReadyText = "Ready";
    public const string NotReadyText = "Not ready";

    public IMatchController matchController;

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

    public GameMode gameMode;

    public List<Transform> playersPositions;
    public List<Material> playersMaterials;

    public IBallController ballController;
    public ConnectionManager connectionManager;
    public ScoreHandler scoreHandler;
    public CountdownHandler countdownHandler;

    public GameObject winnerPanel;
    public GameObject menuPanel;
    public GameObject waitingForOpponentPanel;

    public GameObject leaveButton;
    public GameObject startButton;
    public GameObject readyButton;
    public TextMeshProUGUI readyButtonText;

    public GameObject ballPrefab;
    public GameObject networkBallPrefab;
    public GameObject playerPrefab;
    public GameObject aiPlayerPrefab;

    public NetworkObject[] networkClassicModePrefabs;
    public NetworkObject networkAccuracyModePrefab;

    public GameObject[] classicModePrefabs;
    public GameObject accuracyModePrefab;

    public TextMeshProUGUI debugText;

    public bool debugMode;
    #endregion

    public void ToggleDebugMode(bool newValue)
    {
        debugMode = newValue;
    }

    public void SetDebugText(string text)
    {
        debugText.text = text;
    }

    public void SetUpAIMatchController()
    {
        matchController = gameObject.AddComponent<AIMatchController>();
        var pum = gameObject.AddComponent<Singleplayer.PowerUpsManager>();
        pum.classicModePrefabs = classicModePrefabs;
        pum.accuracyModePrefab = accuracyModePrefab;
    }

    public void SetUpNetworkMatchController()
    {
        matchController = gameObject.AddComponent<NetworkMatchController>();
        var pum = gameObject.AddComponent<Multiplayer.PowerUpsManager>();
        pum.classicModePrefabs = networkClassicModePrefabs;
        pum.accuracyModePrefab = networkAccuracyModePrefab;
    }

    public void EnterTheGame()
    {
        matchController.EnterMatch();
    }

    public void LoadGameMode()
    {
        gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
        GameObject.Find("GameModeDropdown").GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("GameMode");
    }
    public void ResetReadyState()
    {
        readyButtonText.text = ReadyText;
        readyButton.SetActive(true);
    }

    public void QuitToMenu()
    {
        menuPanel.SetActive(true);
        leaveButton.SetActive(false);
        readyButton.SetActive(false);
        startButton.SetActive(false);
        countdownHandler.ResetCountdown();
        matchController.ExitMatch();
    }

    public void StartRound()
    {
        matchController.StartRound();
    }

    public void PrepareForRound()
    {
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void OnReadyButtonClicked()
    {
        (matchController as NetworkMatchController).OnReadyButtonClicked();
    }
}