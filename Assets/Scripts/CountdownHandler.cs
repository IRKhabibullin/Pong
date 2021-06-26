using UnityEngine;
using TMPro;
using System.Collections;
using MLAPI;
using MLAPI.NetworkVariable;

public class CountdownHandler : NetworkBehaviour {
	private int countDownTime = 3;
    private NetworkVariable<string> currentCountdownText = new NetworkVariable<string>();
	[SerializeField] private TextMeshProUGUI countDownText;

    void Start()
    {
        currentCountdownText.OnValueChanged += CountdownUpdated;
    }

    public void CountdownUpdated(string previousValue, string newValue)
    {
        countDownText.text = currentCountdownText.Value;
    }

    public IEnumerator CountDown()
    {
        int counter = countDownTime;
        while (counter > 0)
        {
            currentCountdownText.Value = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        currentCountdownText.Value = "";
    }
}
