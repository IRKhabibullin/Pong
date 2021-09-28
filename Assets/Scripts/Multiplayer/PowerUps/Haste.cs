using MLAPI.Messaging;
using UnityEngine;

namespace Multiplayer
{
    /// <summary>
    /// Buff, speeding up the ball
    /// </summary>
    public class Haste : AbstractPowerUp
    {

        [SerializeField] private float speedMultiplier;

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
            target.GetComponent<TrailRenderer>().emitting = true;
            target.GetComponent<BallController>().ChangeMaterialClientRpc("hasted");
        }

        public override void RemoveBuff()
        {
            if (target != null)
            {
                target.GetComponent<TrailRenderer>().emitting = false;
                target.GetComponent<BallController>().ChangeMaterialClientRpc("normal");
                target.GetComponent<BallController>().Velocity.Value = target.GetComponent<Rigidbody>().velocity / speedMultiplier;
            }
            base.RemoveBuff();
        }
    }
}
