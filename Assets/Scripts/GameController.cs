using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Singleplayer;
using Multiplayer;
using MLAPI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region Variables
    public const string ReadyText = "Ready";
    public const string NotReadyText = "Not ready";

    public IMatchController matchController;
    public Component powerUpManager;

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
    public string controlsType;  // default, alternative

    public List<Transform> playersPositions;
    public List<Material> playersMaterials;

    public IBallController ballController;
    public ConnectionManager connectionManager;
    public ScoreHandler scoreHandler;
    public CountdownHandler countdownHandler;
    public Slider movementSlider;
    public Slider rotationSlider;
    public GameObject controlLevers;
    public TextMeshProUGUI controlsToggleText;

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

    private void Start()
    {
        controlsType = PlayerPrefs.GetString("ControlsType", "alternative");
        controlsToggleText.text = controlsType;
        if (controlsType == "default")
        {
            movementSlider.gameObject.SetActive(false);
            rotationSlider.gameObject.SetActive(false);
            controlLevers.SetActive(false);
        }
        else
        {
            movementSlider.gameObject.SetActive(true);
            rotationSlider.gameObject.SetActive(true);
            controlLevers.SetActive(true);
        }
    }

    #region Toggles
    public void ToggleDebugMode(bool newValue)
    {
        debugMode = newValue;
    }

    public void ToggleControlsType()
    {
        string controlsType = PlayerPrefs.GetString("ControlsType");
        // also need to enable or disable controls
        if (controlsType == "alternative")
        {
            controlsType = "default";
            movementSlider.gameObject.SetActive(false);
            rotationSlider.gameObject.SetActive(false);
            controlLevers.SetActive(false);
        }
        else
        {
            controlsType = "alternative";
            movementSlider.gameObject.SetActive(true);
            rotationSlider.gameObject.SetActive(true);
            controlLevers.SetActive(true);
        }
        controlsToggleText.text = controlsType;
        PlayerPrefs.SetString("ControlsType", controlsType);
    }
    #endregion

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
        powerUpManager = pum;
    }

    public void SetUpNetworkMatchController()
    {
        matchController = gameObject.AddComponent<NetworkMatchController>();
        var pum = gameObject.AddComponent<Multiplayer.PowerUpsManager>();
        pum.classicModePrefabs = networkClassicModePrefabs;
        pum.accuracyModePrefab = networkAccuracyModePrefab;
        powerUpManager = pum;
    }

    public void EnterTheGame()
    {
        matchController.EnterMatch();
        controlsType = PlayerPrefs.GetString("ControlsType", "alternative");
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
        Destroy(matchController as Component);
        Destroy(powerUpManager);
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
        Debug.Log($"OnReadyButtonClicked {matchController}");
        (matchController as NetworkMatchController).OnReadyButtonClicked();
    }
}