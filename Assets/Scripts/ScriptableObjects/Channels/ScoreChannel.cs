using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Score channel")]
public class ScoreChannel : ScriptableObject
{
    public event Action<GameScoreData> OnGameScoreChanged;
    public event Action<BoardSide> OnWinConditionReached;

    public void RaiseOnChangedEvent(GameScoreData data)
    {
        OnGameScoreChanged?.Invoke(data);
    }

    public void RaiseOnWinConditionReachedEvent(BoardSide winnerSide)
    {
        OnWinConditionReached?.Invoke(winnerSide);
    }

    public void ClearSubscriptions()
    {
        OnGameScoreChanged = null;
        OnWinConditionReached = null;
    }
}
