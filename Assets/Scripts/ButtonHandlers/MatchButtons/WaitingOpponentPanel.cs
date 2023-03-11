using TMPro;
using UnityEngine;

public class WaitingOpponentPanel : MonoBehaviour
{
    [SerializeField] private GameObject waitingOpponentPanel;
    [SerializeField] private TextMeshProUGUI ipText;

    private void EnablePanel()
    {
        waitingOpponentPanel.SetActive(true);
        ipText.text = ConnectionManager.GetLocalIPAddress().ToString();
    }

    private void DisablePanel()
    {
        waitingOpponentPanel.SetActive(false);
    }
    
    private void OnEnable()
    {
        EventsManager.LobbyChannel.OnHostButtonPressed += EnablePanel;
        EventsManager.MatchmakingChannel.OnOpponentJoined += DisablePanel;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance)
            return;
        
        EventsManager.LobbyChannel.OnHostButtonPressed -= EnablePanel;
        EventsManager.MatchmakingChannel.OnOpponentJoined -= DisablePanel;
    }
}