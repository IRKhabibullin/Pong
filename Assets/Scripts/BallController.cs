using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using System;
using UnityEngine;
using UnityEngine.Events;

public class BallController : NetworkBehaviour
{
    [Serializable]
    public class BallEvent : UnityEvent<GameObject> { }
    public BallEvent touchdownEvent;
    public BallEvent platformTouchEvent;

    private GameController _gc;

    public NetworkVariableVector3 Velocity = new NetworkVariableVector3();

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LayerMask backWallsLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float speed;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hastedMaterial;

    void Start()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        touchdownEvent.AddListener(_gc.FinishRound);
        platformTouchEvent.AddListener(_gc.PlatformTouchHandler);
        _rb = GetComponent<Rigidbody>();
        Velocity.OnValueChanged += OnVelocityChanged;
    }

    private void OnVelocityChanged(Vector3 previousValue, Vector3 newValue)
    {
        _rb.velocity = newValue;
    }

    void OnCollisionEnter(Collision collision) {
        if (!pongManager.IsServer) return;
        if (LayerMask.Equals(collision.gameObject.layer, backWallsLayer)) {
            touchdownEvent.Invoke(collision.gameObject);
        }
        if (LayerMask.Equals(collision.gameObject.layer, platformLayer)) {
            platformTouchEvent.Invoke(collision.gameObject);
        }
    }

    #region Server methods
    /// <summary>
    /// Removes velocity and places ball in front of lost player
    /// </summary>
    public void ResetBall() {
        Velocity.Value = Vector3.zero;
        transform.position = _gc.pitcher.GetBallStartPosition();
    }

    /// <summary>
    /// Launches ball in random direction away from pitcher
    /// </summary>
    public void LaunchBall() {
        float x_axis_velocity = UnityEngine.Random.Range(-3*speed/4, 3*speed/4);
        float y_axis_velocity = Mathf.Sqrt(speed*speed - x_axis_velocity*x_axis_velocity) * _gc.pitcher.launchDirection;
        Velocity.Value = new Vector3(x_axis_velocity, y_axis_velocity);
    }

    public void MoveBall(Vector3 position) {
        gameObject.transform.position = position;
    }

    public void StopBall()
    {
        Velocity.Value = Vector3.zero;
    }
    #endregion

    [ClientRpc]
    public void ChangeMaterialClientRpc(string material_key)
    {
        switch (material_key)
        {
            case "normal":
                GetComponent<Renderer>().material = normalMaterial;
                break;
            case "hasted":
                GetComponent<Renderer>().material = hastedMaterial;
                break;
        }
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
