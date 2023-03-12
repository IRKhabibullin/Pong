using System;
using UnityEngine;

namespace ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = "Channels/Matchmaking Channel")]
    public class MatchmakingChannel : BaseChannel
    {
        public event Action OnMatchesListChanged;
        public event Action OnOpponentJoined;
        public event Action OnMatchHosted;
        public event Action<Guid> OnMatchJoined;
        public event Action OnMatchLeft;
        
        public enum ChannelEvent
        {
            OnMatchesListChanged,
            OnOpponentJoined,
            OnMatchHosted,
            OnMatchJoined
        }

        public void RaiseOnMatchesListChangedEvent()
        {
            OnMatchesListChanged?.Invoke();
        }

        public void RaiseOnOpponentJoinedEvent()
        {
            OnOpponentJoined?.Invoke();
        }

        public void RaiseOnMatchJoinedEvent(Guid matchId)
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