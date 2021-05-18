using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

public class PlatformController : NetworkBehaviour
{
	public float speed = 30f;
	public float width = 5f;
	public Vector3 ballPosition;
	public float maxRotateAngle = 45f;
	public float rotateSpeed = 100f;
    public int launchDirection;
    [SerializeField] private Rigidbody pRigidbody;

    public NetworkVariableVector3 MoveSpeed = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    [ServerRpc]
    public void MoveServerRpc(float direction) {
        if (Math.Abs(direction) < 0.1)
        {
            direction = 0;
        }
        MoveSpeed.Value = new Vector3(direction, 0, 0) * speed;
    }

    void FixedUpdate()
    {
        // don't move if reached side walls
        if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(MoveSpeed.Value.x), 0, 0), width, LayerMask.GetMask("SideWall")))
        {
            transform.position += MoveSpeed.Value * Time.fixedDeltaTime;
        }
    }

    public float GetCurrentAngle() {
        return ((transform.localRotation.eulerAngles.z + 180f) % 360f) - 180f;
    }

    public void Rotate(float newAngle) {
        if (newAngle > 0) {
            newAngle = Math.Min(newAngle, maxRotateAngle);
        } else {
            newAngle = Math.Max(newAngle, -maxRotateAngle);
        }
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, newAngle));
    }


    /// <summary> Returns where the ball should be placed on a platform at start of the round </summary>
    public Vector2 GetBallStartPosition() {
    	return new Vector3(transform.position.x, transform.position.y, 0) + ballPosition * launchDirection;
    }

    public void ResetPlatform() {
    	transform.localRotation = Quaternion.identity;
    	transform.position = new Vector3(0f, transform.position.y, transform.position.z);
    }
}
