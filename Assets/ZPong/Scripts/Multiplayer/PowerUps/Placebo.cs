using UnityEngine;

namespace Multiplayer
{
    public class Placebo : AbstractPowerUp
    {
        protected override void OnTriggerEnter(Collider collider)
        {
            if (!IsServer) return;
            if (collider.gameObject.CompareTag("Ball"))
            {
                Destroy(gameObject);
            }
        }
    }
}
