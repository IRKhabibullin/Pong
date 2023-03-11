using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostedMatchPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameName;
    [SerializeField] private Button joinButton;

    private Guid matchId;
    
    public void SetData(MatchData matchData)
    {
        gameName.text = matchData.name;
        matchId = matchData.gameId;
        joinButton.onClick.AddListener(JoinMatch);
    }

    private void JoinMatch()
    {
        EventsManager.MatchmakingChannel.RaiseOnMatchJoined(matchId);
    }
}
