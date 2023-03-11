using System;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject searchPanel;

    private void OnEnable()
    {
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed += HideLobbyPanel;
        EventsManager.MatchmakingChannel.OnMatchJoined += HideLobbyPanel;
        EventsManager.LobbyChannel.OnHostButtonPressed += HideLobbyPanel;
        EventsManager.LobbyChannel.OnHostButtonPressed += HostMatch;
        EventsManager.LobbyChannel.OnFindButtonPressed += FindMatch;
        EventsManager.LobbyChannel.OnFindButtonPressed += ShowSearchPanel;
        EventsManager.MatchChannel.OnExitButtonPressed += ShowLobbyPanel;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed -= HideLobbyPanel;
        EventsManager.MatchmakingChannel.OnMatchJoined -= HideLobbyPanel;
        EventsManager.LobbyChannel.OnHostButtonPressed -= HideLobbyPanel;
        EventsManager.MatchChannel.OnExitButtonPressed -= ShowLobbyPanel;
        EventsManager.LobbyChannel.OnFindButtonPressed -= ShowSearchPanel;
        EventsManager.LobbyChannel.OnHostButtonPressed -= HostMatch;
        EventsManager.LobbyChannel.OnFindButtonPressed -= FindMatch;
    }

    private void HideLobbyPanel(Guid gameId)
    {
        HideLobbyPanel();
    }

    private void HideLobbyPanel()
    {
        lobbyPanel.SetActive(false);
    }

    private void ShowLobbyPanel()
    {
        NetworkDiscovery.Instance.Shutdown();
        lobbyPanel.SetActive(true);
    }

    private void ShowSearchPanel()
    {
        searchPanel.SetActive(true);
    }

    private void HideSearchPanel()
    {
        searchPanel.SetActive(false);
    }

    private void HostMatch()
    {
        NetworkDiscovery.Instance.StartBroadcast(new MatchData(
            NetworkDiscovery.GetCurrentIP().ToString(),
            7777,
            Guid.NewGuid(),
            "Yo its my game",
            "Classic"
        ));
    }

    private void FindMatch()
    {
        NetworkDiscovery.Instance.StartListening();
    }
}
