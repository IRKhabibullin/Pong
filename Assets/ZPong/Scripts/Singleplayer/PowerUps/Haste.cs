using UnityEngine;

namespace Singleplayer
{
    /// <summary>
    /// Buff, speeding up the ball
    /// </summary>
    public class Haste : AbstractPowerUp
    {

        [SerializeField] private float speedMultiplier;

        private GameControllerOld _gc;

        protected override void Start()
        {
            _gc = GameObject.Find("GameManager").GetComponent<GameControllerOld>();
            base.Start();
        }

        /// <summary>
        /// Speeding up the ball on a server
        /// </summary>
        public override void ApplyBuff()
        {
            base.ApplyBuff();
            target = _gc.ballController.gameObject;
            target.GetComponent<TrailRenderer>().emitting = true;
            target.GetComponent<Rigidbody>().velocity = target.GetComponent<Rigidbody>().velocity * speedMultiplier;
            target.GetComponent<BallController>().ChangeMaterial("hasted");
        }

        public override void RemoveBuff()
        {
            if (target != null)
            {
                target.GetComponent<TrailRenderer>().emitting = false;
                target.GetComponent<BallController>().ChangeMaterial("normal");
                target.GetComponent<Rigidbody>().velocity = target.GetComponent<Rigidbody>().velocity / speedMultiplier;
            }
            base.RemoveBuff();
        }
    }
}
