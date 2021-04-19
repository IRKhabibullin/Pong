using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject waitingOpponentText;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Button button;


    /// <summary>
    /// Called when player is ready
    /// </summary>
    public void Ready()
    {
        buttonText.text = "Unready";
        waitingOpponentText.SetActive(true);
    }

    /// <summary>
    /// Called when player is not ready
    /// </summary>
    public void Unready()
    {
        buttonText.text = "Ready";
        waitingOpponentText.SetActive(false);
    }

    public void ReadyToStart()
    {
        buttonText.text = "Start";
        waitingOpponentText.SetActive(false);
    }

    /// <summary>
    /// Called when both players are ready and game is up to start
    /// </summary>
    public void StartRound()
    {
        enabled = false;
    }

    /// <summary>
    /// Called when game is paused or round is finished and players preparing for a new round
    /// </summary>
    public void Pause()
    {
        enabled = true;
        buttonText.text = "Ready";
        buttonImage.enabled = true;
        button.interactable = true;
    }
}
