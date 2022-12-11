using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Match channel")]
public class MatchChannel : ScriptableObject
{
    
    public event Action OnExitButtonPressed;
    public event Action OnMatchPanelLoaded;
    public event Action<string> OnMatchFinished;
    
    public void RaiseExitButtonPressedEvent()
    {
        OnExitButtonPressed?.Invoke();
    }

    public void RaiseOnMatchPanelLoadedEvent()
    {
        OnMatchPanelLoaded?.Invoke();
    }

    public void RaiseOnMatchFinishedEvent(string winnerName)
    {
        OnMatchFinished?.Invoke(winnerName);
    }

    public void ClearSubscriptions()
    {
        OnExitButtonPressed = null;
        OnMatchPanelLoaded = null;
        OnMatchFinished = null;
    }
}
