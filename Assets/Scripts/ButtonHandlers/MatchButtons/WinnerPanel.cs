using TMPro;
using UnityEngine;

public class WinnerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerLabel;
    
    private void ShowWinner(string winnerName)
    {
        winnerLabel.text = $"Winner is {winnerName}";
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        EventsManager.MatchChannel.OnMatchFinished += ShowWinner;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance)
            return;
        
        EventsManager.MatchChannel.OnMatchFinished -= ShowWinner;
    }
}
