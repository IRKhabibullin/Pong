using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Lobby channel")]
public class LobbyChannel : ScriptableObject
{
    public event Action OnPlayWithBotButtonPressed;
    public event Action OnHostButtonPressed;
    public event Action OnFindButtonPressed;

    public void RaisePlayWithBotButtonPressedEvent()
    {
        OnPlayWithBotButtonPressed?.Invoke();
    }

    public void RaiseHostButtonPressedEvent()
    {
        OnHostButtonPressed?.Invoke();
    }

    public void RaiseFindButtonPressedEvent()
    {
        OnFindButtonPressed?.Invoke();
    }

    public void ClearSubscriptions()
    {
        OnPlayWithBotButtonPressed = null;
        OnHostButtonPressed = null;
        OnFindButtonPressed = null;
    }
}