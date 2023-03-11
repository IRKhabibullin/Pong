using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Network channel")]
public class NetworkChannel : ScriptableObject
{
    public event Action<MatchData> OnMatchFound;

    public void RaiseOnMatchFoundEvent(MatchData message)
    {
        OnMatchFound?.Invoke(message);
    }
    
    public void ClearSubscriptions()
    {
        OnMatchFound = null;
    }
}
