using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private GameObject roundPanel;

    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject startButton;
    
    
    private void OnEnable()
    {
        EventsManager.Instance.MatchChannel.OnMatchPanelLoaded += LoadPanel;
        EventsManager.Instance.MatchChannel.OnExitButtonPressed += CleanUp;
        EventsManager.Instance.MatchChannel.OnMatchFinished += DisableRoundPanel;
        EventsManager.Instance.RoundChannel.OnStartButtonPressed += DisableStartButton;
        EventsManager.Instance.BoardChannel.OnBackWallTouched += FinishRound;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.Instance.MatchChannel.OnMatchPanelLoaded -= LoadPanel;
        EventsManager.Instance.MatchChannel.OnExitButtonPressed -= CleanUp;
        EventsManager.Instance.MatchChannel.OnMatchFinished -= DisableRoundPanel;
        EventsManager.Instance.RoundChannel.OnStartButtonPressed -= DisableStartButton;
        EventsManager.Instance.BoardChannel.OnBackWallTouched -= FinishRound;
    }

    private void LoadPanel()
    {
        roundPanel.SetActive(true);
        
        PrepareRound();
    }

    private void PrepareRound()
    {
        if (MatchController.MatchMode == MatchMode.Singleplayer)
        {
            EnableStartButton();
        }
        else
        {
            EnableReadyButton();
        }
    }

    private void CleanUp()
    {
        DisableReadyButton();
        DisableStartButton();
        
        roundPanel.SetActive(false);
    }

    private void FinishRound(BoardSide boardSide)
    {
        EventsManager.Instance.RoundChannel.RaiseOnRoundFinishedEvent();
        
        PrepareRound();
    }

    #region UI

    private void EnableRoundPanel()
    {
        roundPanel.SetActive(true);
    }

    private void DisableRoundPanel(string winnerName)
    {
        CleanUp();
    }

    private void EnableStartButton()
    {
        startButton.SetActive(true);
    }

    private void DisableStartButton()
    {
        startButton.SetActive(false);
    }

    private void EnableReadyButton()
    {
        readyButton.SetActive(true);
    }

    private void DisableReadyButton()
    {
        readyButton.SetActive(false);
    }
    
    #endregion
}
