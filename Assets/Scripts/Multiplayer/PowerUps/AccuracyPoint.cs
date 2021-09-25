using UnityEngine;

namespace Multiplayer
{
    public class AccuracyPoint : AbstractPowerUp
    {
        protected override void OnTriggerEnter(Collider collider)
        {
            if (!IsServer) return;
            if (collider.gameObject.CompareTag("Ball"))
            {
                GameObject.Find("GameManager").GetComponent<GameController>().matchController.PowerUpTouchHandler();
                Destroy(gameObject);
            }
        }
    }
}
