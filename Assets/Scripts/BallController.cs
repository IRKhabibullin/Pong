/*using Mirror;*/
using MLAPI;
using Networking;
using System;
using UnityEngine;
using UnityEngine.Events;

public class BallController : MonoBehaviour {
    [SerializeField] private Rigidbody _rb;
    private LayerMask backWalls;
    public float speed;

    [Serializable]
    public class BallEvent : UnityEvent<GameObject> {}
    public BallEvent touchdownEvent;
    public BallEvent platformTouchEvent;

    /*public override void OnStartServer()
    {
        base.OnStartServer();

        // only simulate ball physics on server
        _rb.simulated = true;
    }*/

    private void Awake()
    {
        var _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        touchdownEvent.AddListener(_gc.FinishRound);
        platformTouchEvent.AddListener(_gc.PlatformTouchHandler);
    }

    void Start() {
        backWalls = LayerMask.NameToLayer("BackWall");
        _rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision) {
        if (!pongManager.IsServer) { return; }
        if (collision.gameObject.layer == backWalls) {
            touchdownEvent.Invoke(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player")) {
            platformTouchEvent.Invoke(collision.gameObject);
        }
    }

    public void ResetBall(Vector3 position) {
        _rb.velocity = Vector3.zero;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    public void LaunchBall() {
        float x_axis_velocity = UnityEngine.Random.Range(-3*speed/4, 3*speed/4);
        float y_axis_velocity = Mathf.Sqrt(speed*speed - x_axis_velocity*x_axis_velocity);
        _rb.velocity = new Vector3(x_axis_velocity, y_axis_velocity);
    }

    public void MoveBall(Vector3 position) {
        gameObject.transform.position = position;
    }

    private PongManager pnm;

    private PongManager pongManager
    {
        get
        {
            if (pnm != null) { return pnm; }
            return pnm = NetworkManager.Singleton as PongManager;
        }
    }
}
