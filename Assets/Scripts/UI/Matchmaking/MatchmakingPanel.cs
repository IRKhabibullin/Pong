using Controllers;
using UnityEngine;

namespace UI
{
    public class MatchmakingPanel : BaseSubscriber
    {
        [SerializeField] private HostedMatchPanel matchInstancePrefab;
        [SerializeField] private Transform matchesContainer;

        private void UpdateMatchesObjects()
        {
            foreach (Transform child in matchesContainer.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var message in MatchmakingController.RelevantMatches)
            {
                var gamePanel = Instantiate(matchInstancePrefab, matchesContainer, false);
                gamePanel.SetData(message);
            }
        }

        public void OnMatchesListChangedHandler()
        {
            UpdateMatchesObjects();
        }
    }
}