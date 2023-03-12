using System;
using System.Collections.Generic;
using UnityEngine;

public enum MatchMode
{
    Singleplayer,
    Multiplayer
}

public class MatchController : MonoBehaviour
{
    public static MatchMode MatchMode { get; private set; }
    
    [SerializeField] private GameObject matchPanel;
    [SerializeField] private ScoreController scoreController;
    
    [SerializeField] private GameObject playerPlatformPrefab;
    [SerializeField] private GameObject botPlatformPrefab;

    [SerializeField] private int winCondition = 3;

    private Dictionary<BoardSide, string> sideNames;

    private Platform player1Platform;
    private Platform player2Platform;

    private void Start()
    {
        sideNames = new Dictionary<BoardSide, string>
        {
            { BoardSide.Blue, "" },
            { BoardSide.Red, "" }
        };
    }

    private void OnEnable()
    {
        EventsManager.Instance.LobbyChannel.OnPlayWithBotButtonPressed += LoadMatchWithBot;
        EventsManager.Instance.LobbyChannel.OnHostButtonPressed += LoadHostedMatch;
        EventsManager.Instance.MatchChannel.OnExitButtonPressed += DisableMatchPanel;
        EventsManager.Instance.ScoreChannel.OnWinConditionReached += FinishMatch;
        EventsManager.Instance.LobbyChannel.OnFindButtonPressed += JoinHostedMatch;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.Instance.LobbyChannel.OnPlayWithBotButtonPressed -= LoadMatchWithBot;
        EventsManager.Instance.LobbyChannel.OnHostButtonPressed -= LoadHostedMatch;
        EventsManager.Instance.MatchChannel.OnExitButtonPressed -= DisableMatchPanel;
        EventsManager.Instance.ScoreChannel.OnWinConditionReached -= FinishMatch;
        EventsManager.Instance.LobbyChannel.OnFindButtonPressed -= JoinHostedMatch;
    }

    private void LoadMatchWithBot()
    {
        matchPanel.gameObject.SetActive(true);
        MatchMode = MatchMode.Singleplayer;

        sideNames[BoardSide.Blue] = "<Player name>";
        sideNames[BoardSide.Red] = "Benjamin bot";
        
        scoreController.InitScore(winCondition);
        
        player1Platform = LoadPlayerPlatform().GetComponent<Platform>();
        player2Platform = LoadBotPlatform().GetComponent<Platform>();
        
        EventsManager.Instance.MatchChannel.RaiseOnMatchPanelLoadedEvent();
    }

    private void FinishMatch(BoardSide winnerSide)
    {
        EventsManager.Instance.MatchChannel.RaiseOnMatchFinishedEvent(sideNames[winnerSide]);
    }

    private GameObject LoadPlayerPlatform()
    {
        return Instantiate(playerPlatformPrefab, new Vector3(0, -35, 0), Quaternion.identity);
    }

    private GameObject LoadBotPlatform()
    {
        return Instantiate(botPlatformPrefab, new Vector3(0, 35, 0), Quaternion.identity);
    }

    private void DisableMatchPanel()
    {
        if (MatchMode == MatchMode.Singleplayer)
        {
            Destroy(player1Platform.gameObject);
            Destroy(player2Platform.gameObject);
        }
        else
        {
            ConnectionManager.Instance.Leave();
        }

        matchPanel.gameObject.SetActive(false);
    }

    private void LoadHostedMatch()
    {
        matchPanel.gameObject.SetActive(true);
        MatchMode = MatchMode.Multiplayer;

        ConnectionManager.Instance.Host();

        // sideNames[BoardSide.Blue] = "<Player name>";
        // sideNames[BoardSide.Red] = "Benjamin bot";
        
        scoreController.InitScore(winCondition);

        // EventsManager.MatchChannel.RaiseOnMatchPanelLoadedEvent();
    }

    private void JoinHostedMatch()
    {
        matchPanel.gameObject.SetActive(true);
        MatchMode = MatchMode.Multiplayer;
        // ConnectionManager.Instance.Join();
        scoreController.InitScore(winCondition);
    }
}
