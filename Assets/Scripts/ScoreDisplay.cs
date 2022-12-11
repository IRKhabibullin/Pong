using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winCondition;
    [SerializeField] private TextMeshProUGUI player1Score;
    [SerializeField] private TextMeshProUGUI player2Score;

    private void OnEnable()
    {
        EventsManager.ScoreChannel.OnGameScoreChanged += UpdateScores;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.ScoreChannel.OnGameScoreChanged -= UpdateScores;
    }
    
    private void UpdateScores(GameScoreData data)
    {
        winCondition.text = $"bo {data.winCondition * 2 - 1}";
        
        player1Score.text = $"{data.GetSideScore(BoardSide.Red)}";
        player2Score.text = $"{data.GetSideScore(BoardSide.Blue)}";
    }
}
