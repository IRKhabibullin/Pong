using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class MatchmakingController : MonoBehaviour
    {
        public static IEnumerable<MatchData> RelevantMatches => relevantMatches.Keys.AsEnumerable();
        
        [SerializeField] private float messageLifetime;
    
        private static readonly Dictionary<MatchData, float> relevantMatches = new();
        private IEnumerator trackMatchesEnumerator;
        private readonly WaitForSeconds trackingPeriodWait = new(1);

        private static void AddFoundMatch(MatchData matchData)
        {
            // supposed that match does not change its settings while being broadcasted
            // UI must not allow changing match settings if host already created match
            if (relevantMatches.ContainsKey(matchData))
            {
                relevantMatches[matchData] = Time.time;
            }
            else
            {
                relevantMatches.Add(matchData, Time.time);
                EventsManager.Instance.MatchmakingChannel.RaiseOnMatchesListChangedEvent();
            }
        
            Debug.Log($"Relevant {relevantMatches.Count} matches");
        }

        private void StartTrackingMatches()
        {
            trackMatchesEnumerator = TrackMatchesEnumerator();
            StartCoroutine(trackMatchesEnumerator);
        }

        /// <summary>
        /// Removes matches that aren't broadcasted for some time
        /// </summary>
        private IEnumerator TrackMatchesEnumerator()
        {
            var matchesToRemove = relevantMatches.Keys
                .Where(matchId => Time.time - relevantMatches[matchId] > messageLifetime).ToList();
        
            foreach (var match in matchesToRemove)
            {
                relevantMatches.Remove(match);
            }

            if (matchesToRemove.Count > 0)
            {
                EventsManager.Instance.MatchmakingChannel.RaiseOnMatchesListChangedEvent();
            }

            yield return trackingPeriodWait;
        }

        #region Event handlers

        public void OnFindButtonPressedHandler()
        {
            StartTrackingMatches();
        }

        public void OnMatchFoundHandler(MatchData matchData)
        {
            AddFoundMatch(matchData);
        }

        #endregion

        private void OnEnable()
        {
            EventsManager.SetCallbacks(this);
        }

        private void OnDisable()
        {
            EventsManager.ResetCallbacks(this);
        }
    }
}