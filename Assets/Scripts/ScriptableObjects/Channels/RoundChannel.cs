using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Round channel")]
public class RoundChannel : BaseChannel
{
    #region UI events
    
    public event Action OnStartButtonPressed;
    public event Action OnReadyButtonPressed;
    public event Action OnPauseButtonPressed;
    
    public void RaiseReadyButtonPressedEvent()
    {
        OnReadyButtonPressed?.Invoke();
    }
    
    public void RaiseStartButtonPressedEvent()
    {
        OnStartButtonPressed?.Invoke();
    }

    public void RaisePauseButtonPressedEvent()
    {
        OnPauseButtonPressed?.Invoke();
    }
    
    #endregion
    
    #region Ingame events

    public event Action OnCountdownFinished;
    public event Action OnRoundFinished;

    public void RaiseOnCountdownFinishedEvent()
    {
        OnCountdownFinished?.Invoke();
    }

    public void RaiseOnRoundFinishedEvent()
    {
        OnRoundFinished?.Invoke();
    }

    #endregion

    public void ClearSubscriptions()
    {
        OnReadyButtonPressed = null;
        OnStartButtonPressed = null;
        OnPauseButtonPressed = null;
        
        OnCountdownFinished = null;
        OnRoundFinished = null;
    }
}
