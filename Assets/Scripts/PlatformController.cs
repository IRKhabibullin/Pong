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
    [SerializeField] private Rigidbody pRigidbody;
    private PlayerController _pc;

    private void Start()
    {
        _pc = GetComponent<PlayerController>();
    }

    public NetworkVariableVector3 MoveSpeed = new NetworkVariableVector3(new NetworkVariableSettings
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
            MoveSpeed.Value = new Vector3(direction, 0, 0) * speed;
        }
    }

    void FixedUpdate()
    {
        pRigidbody.velocity = MoveSpeed.Value;
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
    	return new Vector3(transform.position.x, transform.position.y, 0) + ballPosition;
    }

    public void ResetPlatform() {
    	transform.localRotation = Quaternion.identity;
    	transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        pRigidbody.velocity = Vector3.zero;
    }
}
