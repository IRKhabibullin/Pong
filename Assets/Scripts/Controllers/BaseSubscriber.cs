using UnityEngine;

public class BaseSubscriber : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.SetCallbacks(this);
    }

    private void OnDisable()
    {
        EventsManager.ResetCallbacks(this);
    }
}