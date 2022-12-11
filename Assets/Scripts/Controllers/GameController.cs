using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    private void OnEnable()
    {
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed += CreateBall;
        
        EventsManager.BoardChannel.OnPlatformTouched += TriggerPowerUp;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.LobbyChannel.OnPlayWithBotButtonPressed -= CreateBall;
        
        EventsManager.BoardChannel.OnPlatformTouched -= TriggerPowerUp;
    }


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
