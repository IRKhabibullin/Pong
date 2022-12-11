using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class BallController : NetworkBehaviour, IBallController
    {
        public UnityEvent<string> backWallTouchEvent;
        public UnityEvent<GameObject> platformTouchEvent;

        private IMatchController _mc;

        public NetworkVariable<Vector3> Velocity = new NetworkVariable<Vector3>();
        public Rigidbody Rb { get; set; }

        [SerializeField] private float speed;
        private LayerMask backWallsLayer;
        private LayerMask platformLayer;

        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material hastedMaterial;
        [SerializeField] private Material player1Material;
        [SerializeField] private Material player2Material;

        void Start()
        {
            backWallsLayer = LayerMask.NameToLayer("BackWall");
            platformLayer = LayerMask.NameToLayer("Platform");

            _mc = GameObject.Find("GameManager").GetComponent<GameControllerOld>().matchController;
            backWallTouchEvent = new UnityEvent<string>();
            backWallTouchEvent.AddListener(_mc.BackWallTouchHandler);
            platformTouchEvent = new UnityEvent<GameObject>();
            platformTouchEvent.AddListener(_mc.PlatformTouchHandler);
            Rb = GetComponent<Rigidbody>();
            Velocity.OnValueChanged += OnVelocityChanged;
        }

        private void OnVelocityChanged(Vector3 previousValue, Vector3 newValue)
        {
            Rb.velocity = newValue;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (backWallsLayer.value == collision.gameObject.layer)
            {
                backWallTouchEvent.Invoke(collision.gameObject.tag);
            }
            if (platformLayer.value == collision.gameObject.layer)
            {
                platformTouchEvent.Invoke(collision.gameObject);
            }
        }

        #region Server methods
        /// <summary>
        /// Removes velocity and places ball in front of lost player
        /// </summary>
        public void ResetBall()
        {
            Velocity.Value = Vector3.zero;
            transform.position = _mc.Pitcher.GetBallStartPosition();
        }

        /// <summary>
        /// Launches ball in random direction away from pitcher
        /// </summary>
        public void LaunchBall()
        {
            float x_axis_velocity = Random.Range(-3 * speed / 4, 3 * speed / 4);
            float y_axis_velocity = Mathf.Sqrt(speed * speed - x_axis_velocity * x_axis_velocity) * _mc.Pitcher.LaunchDirection;
            Velocity.Value = new Vector3(x_axis_velocity, y_axis_velocity);
        }

        public void MoveBall(Vector3 position)
        {
            gameObject.transform.position = position;
        }

        public void StopBall()
        {
            Velocity.Value = Vector3.zero;
        }
        #endregion

        public void ChangeMaterial(string material_key)
        {
            ChangeMaterialClientRpc(material_key);
        }

        [ClientRpc]
        public void ChangeMaterialClientRpc(string material_key)
        {
            switch (material_key)
            {
                // for classic game mode
                case "normal":
                    GetComponent<Renderer>().material = normalMaterial;
                    break;
                case "hasted":
                    GetComponent<Renderer>().material = hastedMaterial;
                    break;
                // for accuracy game mode
                case "Player1":
                    GetComponent<Renderer>().material = player1Material;
                    break;
                case "Player2":
                    GetComponent<Renderer>().material = player2Material;
                    break;
            }
        }
    }
}
