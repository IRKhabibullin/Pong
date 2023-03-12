using UnityEngine;

public class SingleplayerRoundCreator : MonoBehaviour
{
    [SerializeField] private GameObject botPlatformPrefab;
    
    private void CreateBot()
    {
        var bot = Instantiate(botPlatformPrefab, new Vector3(0, 35, 0), Quaternion.identity);
    }
    
    private void OnEnable()
    {
        EventsManager.Instance.LobbyChannel.OnPlayWithBotButtonPressed += CreateBot;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.Instance.LobbyChannel.OnPlayWithBotButtonPressed -= CreateBot;
    }
}
