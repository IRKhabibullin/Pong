using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using System;
using UnityEngine;
using UnityEngine.Events;

public class BallController : NetworkBehaviour {
    [SerializeField] private Rigidbody _rb;
    private LayerMask backWalls;
    public float speed;
    public NetworkVariableVector3 Velocity = new NetworkVariableVector3();

    [Serializable]
    public class BallEvent : UnityEvent<GameObject> {}
    public BallEvent touchdownEvent;
    public BallEvent platformTouchEvent;

    [SerializeField] private Material[] materials = new Material[2];

    private void Awake()
    {
        var _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        touchdownEvent.AddListener(_gc.FinishRound);
        platformTouchEvent.AddListener(_gc.PlatformTouchHandler);
    }

    void Start() {
        backWalls = LayerMask.NameToLayer("BackWall");
        _rb = GetComponent<Rigidbody>();
        Velocity.OnValueChanged += OnVelocityChanged;
    }

    private void OnVelocityChanged(Vector3 previousValue, Vector3 newValue)
    {
        _rb.velocity = newValue;
    }

    void OnCollisionEnter(Collision collision) {
        if (!pongManager.IsServer) { return; }
        if (collision.gameObject.layer == backWalls) {
            touchdownEvent.Invoke(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2")) {
            platformTouchEvent.Invoke(collision.gameObject);
        }
    }

    public void ResetBall(Vector3 position) {
        Velocity.Value = Vector3.zero;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    public void LaunchBall(int direction) {
        float x_axis_velocity = UnityEngine.Random.Range(-3*speed/4, 3*speed/4);
        float y_axis_velocity = Mathf.Sqrt(speed*speed - x_axis_velocity*x_axis_velocity) * direction;
        Velocity.Value = new Vector3(x_axis_velocity, y_axis_velocity);
    }

    public void MoveBall(Vector3 position) {
        gameObject.transform.position = position;
    }

    public void StopBall()
    {
        Velocity.Value = Vector3.zero;
    }

    [ClientRpc]
    public void ChangeMaterialClientRpc(int material_key)
    {
        GetComponent<Renderer>().material = materials[material_key];
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
