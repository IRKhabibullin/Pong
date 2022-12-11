using UnityEngine;

public class Ball : MonoBehaviour
{
    private float currentSpeed;
    private AudioSource hitSoundAudio;
    private Rigidbody _rigidbody;

    private void Launch()
    {
        _rigidbody.velocity = new Vector3(currentSpeed, currentSpeed, 0);
    }

    private void Reset()
    {
        transform.position = new Vector3(0, -32, 0);
        transform.localScale = Vector3.one * GlobalSettings.Ball.Size;
        currentSpeed = GlobalSettings.Ball.DefaultSpeed;
        _rigidbody.velocity = Vector3.zero;
    }

    private void Start()
    {
        hitSoundAudio = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
        Reset();
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitSoundAudio.Play();
        
        if (GlobalSettings.LayerIncluded(GlobalSettings.Settings.BackWallLayer, collision.gameObject.layer))
        {
            EventsManager.BoardChannel.RaiseBackWallTouchEvent(GlobalSettings.GetSideByTag(collision.gameObject.tag));
        }
        if (GlobalSettings.LayerIncluded(GlobalSettings.Settings.PlatformLayer, collision.gameObject.layer))
        {
            EventsManager.BoardChannel.RaisePlatformTouchEvent(GlobalSettings.GetSideByTag(collision.gameObject.tag));
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventsManager.RoundChannel.OnCountdownFinished += Launch;
        EventsManager.RoundChannel.OnRoundFinished += Reset;

        EventsManager.MatchChannel.OnExitButtonPressed += Despawn;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance) return;
        
        EventsManager.RoundChannel.OnCountdownFinished -= Launch;
        EventsManager.RoundChannel.OnRoundFinished -= Reset;
        
        EventsManager.MatchChannel.OnExitButtonPressed -= Despawn;
    }
}
