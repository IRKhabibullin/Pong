using UnityEngine;

namespace Singleplayer
{
    public class Placebo : AbstractPowerUp
    {
        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Ball"))
            {
                Destroy(gameObject);
            }
        }
    }
}
