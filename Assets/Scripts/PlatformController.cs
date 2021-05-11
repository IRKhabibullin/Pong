using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

public class PlatformController : NetworkBehaviour
{
	public float speed = 30f;
	public float width = 5f;
	public Vector2 ballPosition;
	public float maxRotateAngle = 45f;
	public float rotateSpeed = 100f;
    /*public Vector2 moveSpeed = Vector2.zero;*/
    public Rigidbody2D rigidbody2d;
    private PlayerController _pc;

    private void Start()
    {
        _pc = GetComponent<PlayerController>();
    }

    public NetworkVariableVector2 MoveSpeed = new NetworkVariableVector2(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    [ServerRpc]
    public void MoveServerRpc(float direction) {
        /*TODO ball loses y-axis velocity if platform is frozen in x-axis rotation.*/
        /*if (!isLocalPlayer)
	    {
		    return;
	    }*/
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Math.Sign(direction), 0), width, LayerMask.GetMask("SideWall"));
        // don't move if reached side walls
        if (hit.collider is null) {
	    	//transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
            MoveSpeed.Value = new Vector2(direction, 0) * speed;
        }
    }

    void FixedUpdate()
    {
        rigidbody2d.velocity = MoveSpeed.Value;
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
    	return new Vector2(transform.position.x, transform.position.y) + ballPosition;
    }

    public void ResetPlatform() {
    	transform.localRotation = Quaternion.identity;
    	transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        rigidbody2d.velocity = Vector2.zero;
    }
}
