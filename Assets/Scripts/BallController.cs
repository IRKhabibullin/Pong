﻿using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.Events;

public class BallController : NetworkBehaviour
{
    public UnityEvent<string> backWallTouchEvent;
    public UnityEvent<GameObject> platformTouchEvent;

    private GameController _gc;

    public NetworkVariableVector3 Velocity = new NetworkVariableVector3();

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LayerMask backWallsLayer;
    [SerializeField] private LayerMask platformLayer;

    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hastedMaterial;
    [SerializeField] private Material player1Material;
    [SerializeField] private Material player2Material;

    void Start()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        backWallTouchEvent.AddListener(_gc.BackWallTouchHandler);
        platformTouchEvent.AddListener(_gc.PlatformTouchHandler);
        _rb = GetComponent<Rigidbody>();
        Velocity.OnValueChanged += OnVelocityChanged;
    }

    private void OnVelocityChanged(Vector3 previousValue, Vector3 newValue)
    {
        _rb.velocity = newValue;
    }

    void OnCollisionEnter(Collision collision) {
        if (!NetworkManager.Singleton.IsServer) return;
        if ((backWallsLayer.value & (1 << collision.gameObject.layer)) > 0) {
            backWallTouchEvent.Invoke(collision.gameObject.tag);
        }
        if ((platformLayer.value & (1 << collision.gameObject.layer)) > 0) {
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
        float x_axis_velocity = Random.Range(-3*speed/4, 3*speed/4);
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
            // for classic game mode
            case "normal":
                GetComponent<Renderer>().material = normalMaterial;
                break;
            case "hasted":
                GetComponent<Renderer>().material = hastedMaterial;
                break;
            // for accuracy game mode
            case "Player1":
                GetComponent<Renderer>().material = player1Material;
                break;
            case "Player2":
                GetComponent<Renderer>().material = player2Material;
                break;
        }
    }
}
