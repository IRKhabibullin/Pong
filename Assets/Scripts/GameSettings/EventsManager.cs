using System;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    [SerializeField] private BoardChannel boardChannel;
    [SerializeField] private ScoreChannel scoreChannel;
    [SerializeField] private RoundChannel roundChannel;
    [SerializeField] private LobbyChannel lobbyChannel;
    [SerializeField] private MatchChannel matchChannel;

    public static BoardChannel BoardChannel => Instance.boardChannel;
    public static ScoreChannel ScoreChannel => Instance.scoreChannel;
    public static RoundChannel RoundChannel => Instance.roundChannel;
    public static LobbyChannel LobbyChannel => Instance.lobbyChannel;
    public static MatchChannel MatchChannel => Instance.matchChannel;

    #region singleton setup
    public static EventsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventsManager>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<EventsManager>();
                }
            }
            return _instance;
        }
    }

    public static bool HasInstance => _instance != null;
    
    private static EventsManager _instance;

    void Awake()
    {
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    #endregion

    private void OnDestroy()
    {
        boardChannel.ClearSubscriptions();
        scoreChannel.ClearSubscriptions();
        roundChannel.ClearSubscriptions();
        lobbyChannel.ClearSubscriptions();
        matchChannel.ClearSubscriptions();
    }
}
