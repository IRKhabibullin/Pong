using System;
using UnityEngine;

public class LobbyController : BaseSubscriber
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject searchPanel;

    #region Event handlers
    
    public void OnPlayWithBotButtonPressedHandler()
    {
        HideLobbyPanel();
    }

    public void OnMatchJoinedHandler()
    {
        HideLobbyPanel();
    }

    public void OnHostButtonPressedHandler()
    {
        HideLobbyPanel();
        HostMatch();
    }

    public void OnFindButtonPressedHandler()
    {
        FindMatch();
        ShowSearchPanel();
    }

    public void OnExitButtonPressedHandler()
    {
        ShowLobbyPanel();
    }
    
    #endregion

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
