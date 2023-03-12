using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Lobby channel")]
public class LobbyChannel : BaseChannel
{
    public event Action OnPlayWithBotButtonPressed;
    public event Action OnHostButtonPressed;
    public event Action OnFindButtonPressed;

    public static Action OnSomethingHappened;

    public Action OnConstHappened;

    public class ChannelEvent
    {
        public const int OnPlayWithBotButtonPressed = 0;
        public const int OnHostButtonPressed = 1;
        public const int OnFindButtonPressed = 2;
        public const int OnConstHappened = 3;
    }

    private Dictionary<int, Action> events = new()
    {
        { ChannelEvent.OnConstHappened, OnSomethingHappened }
    };

    public Action GetEventAction(int eventName)
    {
        if (!events.ContainsKey(eventName))
            return null;

        return events[eventName];
    }

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