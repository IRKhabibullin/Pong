using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;
using static GameController;

public class PlatformController : NetworkBehaviour
{
	public float maxSpeed = 30f;
	public float width = 5f;
	public Vector3 ballPosition;
	public float maxRotateAngle = 45f;
	public float rotateSpeed = 100f;
    public int launchDirection;
    public GameController _gc;

    public Vector3 mSpeed = Vector3.zero;  // current speed. Used only on server, position on client is synced from server
    public NetworkVariableVector3 mPosition = new NetworkVariableVector3();
    public NetworkVariable<float> mAngle = new NetworkVariable<float>();
    public NetworkVariableQuaternion mRotation = new NetworkVariableQuaternion();

    private void Start()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        mRotation.OnValueChanged += OnRotationChanged;
    }

    private void OnRotationChanged(Quaternion previousValue, Quaternion newValue)
    {
        transform.rotation = mRotation.Value;
    }

    [ServerRpc]
    public void MoveServerRpc(float direction) {
        if (Math.Abs(direction) < 0.1)
        {
            mSpeed = Vector3.zero;
            return;
        }
        mSpeed = new Vector3(direction, 0, 0) * maxSpeed;
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            if (_gc.gameState.Value != GameStates.Play) return;
            if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(mSpeed.x), 0, 0), width, LayerMask.GetMask("SideWall")))
                mPosition.Value += mSpeed * Time.fixedDeltaTime;
            Rotate();
        }
        transform.position = mPosition.Value;
    }

    public float GetCurrentAngle() {
        return ((transform.localRotation.eulerAngles.z + 180f) % 360f) - 180f;
    }

    [ServerRpc]
    public void SetRotationServerRpc(float rotation)
    {
        mAngle.Value = rotation;
    }

    public void Rotate()
    {
        var newAngle = GetCurrentAngle() + mAngle.Value * rotateSpeed * Time.deltaTime;
        if (newAngle > 0) {
            newAngle = Math.Min(newAngle, maxRotateAngle);
        } else {
            newAngle = Math.Max(newAngle, -maxRotateAngle);
        }
        mRotation.Value = Quaternion.Euler(new Vector3(0f, 0f, newAngle));
    }

    /// <summary> Returns where the ball should be placed on a platform at start of the round </summary>
    public Vector2 GetBallStartPosition() {
    	return new Vector3(mPosition.Value.x, mPosition.Value.y, 0) + ballPosition * launchDirection;
    }

    public void ResetPlatform()
    {
        transform.localRotation = Quaternion.identity;
    	mPosition.Value = new Vector3(0f, mPosition.Value.y, 0f);
    }
}
