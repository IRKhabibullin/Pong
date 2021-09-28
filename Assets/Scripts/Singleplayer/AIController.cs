using UnityEngine;

public class AIController : MonoBehaviour {

	private IPlatformController _pc;
	private int ballLayer;
	private float lastSetSpeed;
	private (Vector2 left, Vector2 right) rayHits;
    private GameController _gc;
    private IBallController ballController;

    void Start() {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        _pc = gameObject.GetComponent<IPlatformController>();
        ballController = _gc.ballController;
        ballLayer = LayerMask.GetMask("Ball");
        lastSetSpeed = 0f;
    }


    void Update() {
    	// СastObserverRay();
        FollowTheBall();
    }

    private void СastObserverRay() {
        RaycastHit leftRay;
        RaycastHit rightRay;
        bool leftHit;
        bool rightHit;
        leftHit = Physics.Raycast(transform.position + new Vector3(-0.75f, -1.5f, 0), Vector3.down, out leftRay, 100f, ballLayer);
    	rightHit = Physics.Raycast(transform.position + new Vector3(0.75f, -1.5f, 0), Vector3.down, out rightRay, 100f, ballLayer);
    	rayHits = (leftRay.point, rightRay.point);
        if (leftHit && rightHit) {
            if (lastSetSpeed > 0)
            {
                lastSetSpeed = Mathf.Max(lastSetSpeed - 0.01f, 0);
            }
            else if (lastSetSpeed < 0)
            {
                lastSetSpeed = Mathf.Min(lastSetSpeed + 0.01f, 0);
            }
            else
        	    lastSetSpeed = 0f;
        	return;
        }
        if (leftHit) {
        	lastSetSpeed = Mathf.Max(lastSetSpeed - 0.01f, -1f);
        }
        if (rightHit) {
        	lastSetSpeed = Mathf.Min(lastSetSpeed + 0.01f, 1f);
        }
        _pc.SetSpeed(lastSetSpeed);
    }

    private void FollowTheBall()
    {
        var direction = ballController.gameObject.transform.position.x - transform.position.x;
        if (Mathf.Abs(direction) < 2)
        {
            _pc.SetSpeed(0);
        }
        else
        {
            _pc.SetSpeed(Mathf.Sign(direction));
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-0.75f, -1.5f, 0), rayHits.left);
        Gizmos.DrawLine(transform.position + new Vector3(0.75f, -1.5f, 0), rayHits.right);
    }
}
