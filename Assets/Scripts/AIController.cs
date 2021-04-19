using UnityEngine;

public class AIController : MonoBehaviour {

	private PlatformController _pc;
	private int ballLayer;
	private float lastSeenDirection;
	private (Vector2 left, Vector2 right) rayHits;

    void Start() {
        _pc = gameObject.GetComponent<PlatformController>();
        ballLayer = LayerMask.GetMask("Ball");
        lastSeenDirection = 0f;
    }


    void Update() {
    	СastObserverRay();
    }

    private void СastObserverRay() {
    	RaycastHit2D leftRay = Physics2D.Raycast(transform.position + new Vector3(-0.75f, -1.5f, 0f), Vector2.down, 70f, ballLayer);
    	RaycastHit2D rightRay = Physics2D.Raycast(transform.position + new Vector3(0.75f, -1.5f, 0f), Vector2.down, 70f, ballLayer);
    	rayHits = (leftRay.point, rightRay.point);
    	bool leftHit = leftRay.collider != null;
    	bool rightHit = rightRay.collider != null;
        if (leftHit && rightHit) {
        	lastSeenDirection = 0f;
        	return;
        }
        if (leftHit) {
        	lastSeenDirection = -1f;
        }
        if (rightHit) {
        	lastSeenDirection = 1f;
        }
        /*_pc.Move(lastSeenDirection);*/
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-0.75f, -1.5f, 0f), rayHits.left);
        Gizmos.DrawLine(transform.position + new Vector3(0.75f, -1.5f, 0f), rayHits.right);
    }
}
