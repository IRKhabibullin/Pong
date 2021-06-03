using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class AbstractPowerUp : NetworkBehaviour {

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

    protected virtual void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Ball")) {
            ApplyBuff();
            HideAfterTriggerClientRpc();
        }
    }

    [ClientRpc]
    public void HideAfterTriggerClientRpc()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public virtual void ApplyBuff() {
		buffCurTime = 0f;
		applied = true;
    }

    public virtual void RemoveBuff() {
    	Destroy(gameObject);
    }
}
