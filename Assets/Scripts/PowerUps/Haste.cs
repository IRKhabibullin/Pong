using UnityEngine;

public class Haste : AbstractPowerUp {

	public float speedMultiplier;
    public Material normal;
    public Material hasted;

    protected override void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        target = collider.gameObject;
        base.OnTriggerEnter(collider);
    }

    public override void ApplyBuff()
    {
        target.GetComponent<Renderer>().material = hasted;
        Vector3 velocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 newVelocity = new Vector3(velocity.x * speedMultiplier, velocity.y * speedMultiplier, 0);
        target.GetComponent<Rigidbody>().velocity = newVelocity;
        base.ApplyBuff();
    }

    public override void RemoveBuff() {
        if (target != null) {
    	    target.GetComponent<Renderer>().material = normal;
		    Vector3 velocity = target.GetComponent<Rigidbody>().velocity;
		    Vector3 newVelocity = new Vector3(velocity.x / speedMultiplier, velocity.y / speedMultiplier, 0);
    	    target.GetComponent<Rigidbody>().velocity = newVelocity;
        }
        base.RemoveBuff();
    }
}
