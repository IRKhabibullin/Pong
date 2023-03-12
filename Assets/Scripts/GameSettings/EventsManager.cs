using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ScriptableObjects.Channels;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    #region Channels

    [field: SerializeField] public BoardChannel BoardChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public ScoreChannel ScoreChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public RoundChannel RoundChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public LobbyChannel LobbyChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public MatchChannel MatchChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public NetworkChannel NetworkChannel { get; [UsedImplicitly] private set; }
    [field: SerializeField] public MatchmakingChannel MatchmakingChannel { get; [UsedImplicitly] private set; }

    #endregion

    public static void SetCallbacks(object subscriberObject)
    {
        foreach (var field in Instance.GetType().GetRuntimeFields().Where(f => f.FieldType.BaseType == typeof(BaseChannel)))
        {
            ((BaseChannel)field.GetValue(Instance)).SetCallbacks(subscriberObject);
        }
    }

    public static void ResetCallbacks(object subscriberObject)
    {
        if (!HasInstance)
            return;
        
        foreach (var field in Instance.GetType().GetRuntimeFields().Where(f => f.FieldType.BaseType == typeof(BaseChannel)))
        {
            ((BaseChannel)field.GetValue(Instance)).ResetCallbacks(subscriberObject);
        }
    }

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
        BoardChannel.ClearSubscriptions();
        ScoreChannel.ClearSubscriptions();
        RoundChannel.ClearSubscriptions();
        LobbyChannel.ClearSubscriptions();
        MatchChannel.ClearSubscriptions();
        NetworkChannel.ClearSubscriptions();
        MatchmakingChannel.ClearSubscriptions();
    }
}
