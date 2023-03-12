using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private GameScoreData gameScoreData;

    public void InitScore(int winCondition)
    {
        gameScoreData = new GameScoreData(winCondition);

        EventsManager.Instance.ScoreChannel.RaiseOnChangedEvent(gameScoreData);
    }

    private void UpdateScore(BoardSide side)
    {
        gameScoreData.AddScore(side, 1);

        EventsManager.Instance.ScoreChannel.RaiseOnChangedEvent(gameScoreData);

        if (gameScoreData.IsWinConditionReached(out var winnerSide))
        {
            EventsManager.Instance.ScoreChannel.RaiseOnWinConditionReachedEvent(winnerSide);
        }
    }

    private void OnEnable()
    {
        EventsManager.Instance.BoardChannel.OnBackWallTouched += UpdateScore;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;

        EventsManager.Instance.BoardChannel.OnBackWallTouched -= UpdateScore;
    }
}

public class GameScoreData
{
    public int winCondition;
    private Dictionary<BoardSide, int> scores;

    public GameScoreData(int winCondition = 5)
    {
        this.winCondition = winCondition;

        scores = new Dictionary<BoardSide, int>
        {
            { BoardSide.Red, 0 },
            { BoardSide.Blue, 0 }
        };
    }

    public int GetSideScore(BoardSide side)
    {
        return scores[side];
    }

    public void AddScore(BoardSide side, int points)
    {
        scores[side] += points;
    }

    public bool IsWinConditionReached(out BoardSide winnerSide)
    {
        winnerSide = default;
        foreach (var side in scores.Keys.Where(side => scores[side] >= winCondition))
        {
            winnerSide = side;
            return true;
        }

        return false;
    }
}