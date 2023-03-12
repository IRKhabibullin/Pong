using TMPro;
using UnityEngine;

public class WaitingOpponentPanel : BaseSubscriber
{
    [SerializeField] private GameObject waitingOpponentPanel;
    [SerializeField] private TextMeshProUGUI ipText;

    #region Event handlers
    
    public void OnHostButtonPressedHandler()
    {
        EnablePanel();
    }

    public void OnOpponentJoinedHandler()
    {
        DisablePanel();
    }

    #endregion

    private void EnablePanel()
    {
        waitingOpponentPanel.SetActive(true);
        ipText.text = ConnectionManager.GetLocalIPAddress().ToString();
    }

    private void DisablePanel()
    {
        waitingOpponentPanel.SetActive(false);
    }
}