using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;
using static GameController;

public class PlatformController : NetworkBehaviour
{
    public int launchDirection;

    [SerializeField] private float width = 5f;
    [SerializeField] private float maxSpeed = 30f;

	[SerializeField] private Vector3 ballPosition;  // where ball must be placed when player is pitcher
    private GameController _gc;

    public Vector3 mSpeed = Vector3.zero;  // current speed. Used only on server. Position on client is synced from server
    public NetworkVariableVector3 mPosition = new NetworkVariableVector3();  // synced variable for platform position

    public float mAngle = 0;  // current rotation speed. Used only on server. Rotation on client is synced from server
    public NetworkVariableQuaternion mRotation = new NetworkVariableQuaternion();  // synced variable for platform rotation

    private void Awake()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        if (((IsServer || _gc.playWithBot) && _gc.gameState.Value == GameState.Play) || _gc.debugMode)
        {
            // move
            if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(mSpeed.x), 0, 0), width, LayerMask.GetMask("SideWall")))
                mPosition.Value += mSpeed * Time.fixedDeltaTime;
        }
        // sync position on clients
        transform.position = mPosition.Value;
        transform.rotation = mRotation.Value;
    }

    public void SetUp(int side, bool playWithBots = false)
    {
        launchDirection = side == 0 ? 1 : -1;
        mPosition.Value = _gc.playersPositions[side].position;
        if (playWithBots)
            GetComponent<MeshRenderer>().material = _gc.playersMaterials[side];
        else
            SetColorClientRpc(side);
        
    }

    [ServerRpc]
    public void SetRotationServerRpc(float rotation)
    {
        SetRotation(rotation);
    }

    public void SetRotation(float rotation)
    {
        mRotation.Value = Quaternion.Euler(Vector3.forward * rotation);
    }

    [ServerRpc]
    public void SetSpeedServerRpc(float direction)
    {
        SetSpeed(direction);
    }

    public void SetSpeed(float direction)
    {
        if (Math.Abs(direction) < 0.1)
        {
            mSpeed = Vector3.zero;
            return;
        }
        mSpeed = new Vector3(direction, 0, 0) * maxSpeed;
    }

    public float GetCurrentAngle()
    {
        return ((transform.localRotation.eulerAngles.z + 180f) % 360f) - 180f;
    }

    /// <summary>
    /// Returns where the ball should be placed on a platform at start of the round
    /// </summary>
    public Vector2 GetBallStartPosition() {
    	return new Vector3(mPosition.Value.x, mPosition.Value.y, 0) + ballPosition * launchDirection;
    }

    [ClientRpc]
    public void SetColorClientRpc(int playerNumber)
    {
        GetComponent<MeshRenderer>().material = _gc.playersMaterials[playerNumber];
    }

    /// <summary>
    /// Resets position and rotation of platform. Called only on server
    /// </summary>
    public void ResetPlatform()
    {
        mSpeed = Vector3.zero;
        mAngle = 0;
    	mPosition.Value = new Vector3(0f, mPosition.Value.y, 0f);
        mRotation.Value = Quaternion.identity;
    }
}
