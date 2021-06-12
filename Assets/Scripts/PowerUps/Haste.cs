using MLAPI.Messaging;
using UnityEngine;

/// <summary>
/// Buff, speeding up the ball
/// </summary>
public class Haste : AbstractPowerUp {

    [SerializeField] private float speedMultiplier;
    [SerializeField] private Material normal;
    [SerializeField] private Material hasted;

    private GameController _gc;

    protected override void Start()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        base.Start();
    }

    /// <summary>
    /// Speeding up the ball on a server
    /// </summary>
    public override void ApplyBuff()
    {
        base.ApplyBuff();
        target = _gc.ballController.gameObject;
        target.GetComponent<BallController>().Velocity.Value = target.GetComponent<Rigidbody>().velocity * speedMultiplier;
        ApplyBuffClientRpc();
    }

    [ClientRpc]
    public void ApplyBuffClientRpc()
    {
        target = _gc.ballController.gameObject;
        target.GetComponent<Renderer>().material = hasted;
    }

    public override void RemoveBuff() {
        if (target != null) {
    	    target.GetComponent<Renderer>().material = normal;
		    Vector3 velocity = target.GetComponent<Rigidbody>().velocity;
		    Vector3 newVelocity = new Vector3(velocity.x / speedMultiplier, velocity.y / speedMultiplier, 0);
    	    target.GetComponent<BallController>().Velocity.Value = newVelocity;
            RemoveBuffClientRpc();
        }
        base.RemoveBuff();
    }

    [ClientRpc]
    public void RemoveBuffClientRpc()
    {
        target.GetComponent<Renderer>().material = normal;
    }
}
