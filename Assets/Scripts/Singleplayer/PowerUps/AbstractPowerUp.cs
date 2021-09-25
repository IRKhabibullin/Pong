using UnityEngine;

namespace Singleplayer
{
    public class AbstractPowerUp : MonoBehaviour
    {

        [SerializeField] private float buffDuration;
        protected float buffCurTime;

        [SerializeField] private float maxLifeTime;
        protected float lifeTime;

        protected bool applied;
        protected GameObject target;

        protected virtual void Start()
        {
            applied = false;
            lifeTime = 0f;
        }

        protected virtual void Update()
        {
            if (applied)
            {
                buffCurTime += Time.deltaTime;
                if (buffCurTime > buffDuration)
                {
                    RemoveBuff();
                }
            }
            lifeTime += Time.deltaTime;
            if (lifeTime > maxLifeTime && !applied)
            {
                Destroy(gameObject);
            }

        }

        /// <summary>
        /// Collision with ball activates buff
        /// </summary>
        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Ball"))
            {
                ApplyBuff();
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }

        /// <summary>
        /// Buff applying must be called only on server
        /// </summary>
        public virtual void ApplyBuff()
        {
            buffCurTime = 0f;
            applied = true;
        }

        /// <summary>
        /// Buff removing must be called only on server
        /// </summary>
        public virtual void RemoveBuff()
        {
            Destroy(gameObject);
        }
    }
}
