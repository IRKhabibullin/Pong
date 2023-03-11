using System;
using UnityEngine;

namespace ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = "Channels/Matchmaking Channel")]
    public class MatchmakingChannel : ScriptableObject
    {
        public event Action OnMatchesListChanged;
        public event Action OnOpponentJoined;
        public event Action OnMatchHosted;
        public event Action<Guid> OnMatchJoined;
        public event Action OnMatchLeft;

        public void RaiseOnMatchesListChangedEvent()
        {
            OnMatchesListChanged?.Invoke();
        }

        public void RaiseOnOpponentJoinedEvent()
        {
            OnOpponentJoined?.Invoke();
        }

        public void RaiseOnMatchJoined(Guid matchId)
        {
            OnMatchJoined?.Invoke(matchId);
        }
        
        public void ClearSubscriptions()
        {
            OnMatchesListChanged = null;
            OnOpponentJoined = null;
        }
    }
}