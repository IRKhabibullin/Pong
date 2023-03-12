using TMPro;
using UnityEngine;

public class WinnerPanel : BaseSubscriber
{
    [SerializeField] private TextMeshProUGUI winnerLabel;
    
    private void ShowWinner(string winnerName)
    {
        winnerLabel.text = $"Winner is {winnerName}";
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    #region Event handlers

    public void OnMatchFinishedHandler(string winnerName)
    {
        ShowWinner(winnerName);
    }

    #endregion
}
