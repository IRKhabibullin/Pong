using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPanel;
    
    private void OnEnable()
    {
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed += HideLobbyPanel;
        EventsManager.MatchChannel.OnExitButtonPressed += ShowLobbyPanel;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed -= HideLobbyPanel;
        EventsManager.MatchChannel.OnExitButtonPressed -= ShowLobbyPanel;
    }

    private void HideLobbyPanel()
    {
        lobbyPanel.SetActive(false);
    }

    private void ShowLobbyPanel()
    {
        lobbyPanel.SetActive(true);
    }
}
