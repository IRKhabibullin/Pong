using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : AbstractPowerUp {

	public float speedMultiplier;

    protected override void OnTriggerEnter2D(Collider2D collider) {
        target = collider.gameObject;
        base.OnTriggerEnter2D(collider);
    }

    public override void ApplyBuff() {
        target.GetComponent<SpriteRenderer>().color = new Color(255f, 0f, 0f);
        Vector2 velocity = target.GetComponent<Rigidbody2D>().velocity;
        Vector2 newVelocity = new Vector2(velocity.x * speedMultiplier, velocity.y * speedMultiplier);
        target.GetComponent<Rigidbody2D>().velocity = newVelocity;
        base.ApplyBuff();
    }

    public override void RemoveBuff() {
        if (target != null) {
    	    target.GetComponent<SpriteRenderer>().color = new Color(248f, 236f, 88f);
		    Vector2 velocity = target.GetComponent<Rigidbody2D>().velocity;
		    Vector2 newVelocity = new Vector2(velocity.x / speedMultiplier, velocity.y / speedMultiplier);
    	    target.GetComponent<Rigidbody2D>().velocity = newVelocity;
        }
        base.RemoveBuff();
    }
}
