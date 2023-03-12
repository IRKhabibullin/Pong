using UnityEngine;

public class GameController : BaseSubscriber
{
    [SerializeField] private GameObject ballPrefab;

    #region Event handlers

    public void OnPlayWithBotButtonPressedHandler()
    {
        CreateBall();
    }
    
    public void OnPlatformTouchedHandler(BoardSide side)
    {
        TriggerPowerUp(side);
    }

    #endregion

    private void TriggerPowerUp(BoardSide side)
    {
        Debug.Log("Triggering powerups");
    }

    private void CreateBall()
    {
        var ball = Instantiate(ballPrefab);
        // ball.GetComponent<NetworkObject>().Spawn();
    }
}
