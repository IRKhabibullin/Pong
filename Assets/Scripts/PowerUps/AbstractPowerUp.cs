using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractPowerUp : MonoBehaviour {

	public float buffDuration;
    protected float buffCurTime;

    public float maxLifeTime;
	protected float lifeTime;

	protected bool applied;
	protected GameObject target;

    protected virtual void Start() {
		applied = false;
		lifeTime = 0f;
    }

    protected virtual void Update() {
        if (applied) {
            buffCurTime += Time.deltaTime;
            if (buffCurTime > buffDuration) {
                RemoveBuff();
            }
        }
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime && !applied) {
            Destroy(gameObject);
        }
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider) {
    	if (collider.gameObject.name == "Ball") {
    		ApplyBuff();
    		GetComponent<Renderer>().enabled = false;
    		GetComponent<CircleCollider2D>().enabled = false;
    	}
    }

    public virtual void ApplyBuff() {
		buffCurTime = 0f;
		applied = true;
    }

    public virtual void RemoveBuff() {
    	Destroy(gameObject);
    }
}
