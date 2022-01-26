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
    public SettingsController settings;

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
    public GameObject testPlatform;

    public GameObject winnerPanel;
    public GameObject menuPanel;
    public GameObject waitingForOpponentPanel;
    public GameObject rulesClassicPanel;
    public GameObject rulesAccuracyPanel;

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

    public bool debugMode;
    #endregion

    private void Start()
    {
        string language = PlayerPrefs.GetString("Language");
        Debug.Log($"Language is {language}");
        if (language != "")
        {
            StartCoroutine(settings.SetLanguage(language));
        }
        controlsType = PlayerPrefs.GetString("ControlsType", "alternative");
    }

    public void ToggleControls(bool value)
    {
        string controlsType = PlayerPrefs.GetString("ControlsType");
        if (controlsType == "alternative")
        {
            movementSlider.gameObject.SetActive(value);
            rotationSlider.gameObject.SetActive(value);
            movementSlider.value = 0;
            rotationSlider.value = 0;
            controlLevers.SetActive(value);
        }
    }

    public void ToggleControlsInteraction(bool value)
    {
        string controlsType = PlayerPrefs.GetString("ControlsType");
        if (controlsType == "alternative")
        {
            movementSlider.interactable = value;
            rotationSlider.interactable = value;
        }
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
        controlsType = PlayerPrefs.GetString("ControlsType", "default");
    }

    public void SelectGameMode(int modeIndex)
    {
        PlayerPrefs.SetInt("GameMode", modeIndex);
        gameMode = (GameMode)modeIndex;
    }

    public void ShowRules()
    {
        if (gameMode == GameMode.Classic)
            rulesClassicPanel.SetActive(true);
        else
            rulesAccuracyPanel.SetActive(true);
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
        ToggleControls(false);
        ToggleControlsInteraction(false);
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
        (matchController as NetworkMatchController).OnReadyButtonClicked();
    }
}