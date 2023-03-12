using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountdownController : BaseSubscriber
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private const int CountdownTime = 3;
    private readonly WaitForSeconds countdownSecond = new(1);


    [ClientRpc]
    private void UpdateCountdownClientRpc(string value)
    {
        countdownText.text = value;
    }

    private void UpdateCountdown(string value)
    {
        UpdateCountdownClientRpc(value);
    }

    private void StartCountDown()
    {
        StartCoroutine(CountDownEnumerator());
    }

    private void HideCountdown()
    {
        countdownText.text = "";
    }
    
    private IEnumerator CountDownEnumerator()
    {
        var counter = CountdownTime;
        while (counter > 0)
        {
            UpdateCountdown($"{counter}");
            yield return countdownSecond;
            counter--;
        }
        UpdateCountdown("Go!");
        yield return countdownSecond;
        UpdateCountdown("");
        
        EventsManager.Instance.RoundChannel.RaiseOnCountdownFinishedEvent();
    }

    #region Event handlers

    public void OnStartButtonPressedHandler()
    {
        StartCountDown();
    }
    
    public void OnExitButtonPressedHandler()
    {
        HideCountdown();
    }

    #endregion
}
