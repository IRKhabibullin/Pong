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
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed += LoadMatchWithBot;
        EventsManager.MatchChannel.OnExitButtonPressed += DisableMatchPanel;
        EventsManager.ScoreChannel.OnWinConditionReached += FinishMatch;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed -= LoadMatchWithBot;
        EventsManager.MatchChannel.OnExitButtonPressed -= DisableMatchPanel;
        EventsManager.ScoreChannel.OnWinConditionReached -= FinishMatch;
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
        
        EventsManager.MatchChannel.RaiseOnMatchPanelLoadedEvent();
    }

    private void FinishMatch(BoardSide winnerSide)
    {
        EventsManager.MatchChannel.RaiseOnMatchFinishedEvent(sideNames[winnerSide]);
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
        Destroy(player1Platform.gameObject);
        Destroy(player2Platform.gameObject);
        
        matchPanel.gameObject.SetActive(false);
    }
}
