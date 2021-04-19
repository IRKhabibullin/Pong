using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownHandler : MonoBehaviour {
	private int countDownTime = 3;
	private TextMeshProUGUI countDownText;

    void Start() {
        countDownText = GetComponent<TextMeshProUGUI>();
        Reset();
    }

    public void Reset() {
	    countDownText.text = countDownTime.ToString();
    }

    public IEnumerator CountDown()
    {
        int counter = countDownTime;
        while (counter > 0)
        {
            countDownText.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        countDownText.text = "";
    }
}
