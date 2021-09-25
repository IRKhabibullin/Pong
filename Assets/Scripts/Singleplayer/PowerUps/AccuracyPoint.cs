using UnityEngine;

namespace Singleplayer
{
    public class AccuracyPoint : AbstractPowerUp
    {
        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Ball"))
            {
                GameObject.Find("GameManager").GetComponent<GameController>().matchController.PowerUpTouchHandler();
                Destroy(gameObject);
            }
        }
    }
}
