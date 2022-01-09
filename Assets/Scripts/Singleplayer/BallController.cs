using UnityEngine;
using UnityEngine.Events;

namespace Singleplayer
{
    public class BallController : MonoBehaviour, IBallController
    {
        public UnityEvent<string> backWallTouchEvent;
        public UnityEvent<GameObject> platformTouchEvent;
        public Rigidbody Rb { get; set; }
        public AudioSource hitSound;

        private IMatchController _mc;

        [SerializeField] private float speed = 25;
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

            _mc = GameObject.Find("GameManager").GetComponent<GameController>().matchController;
            backWallTouchEvent = new UnityEvent<string>();
            backWallTouchEvent.AddListener(_mc.BackWallTouchHandler);
            platformTouchEvent = new UnityEvent<GameObject>();
            platformTouchEvent.AddListener(_mc.PlatformTouchHandler);
            Rb = GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision)
        {
            hitSound.Play();
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
            Rb.velocity = Vector3.zero;
            transform.position = _mc.Pitcher.GetBallStartPosition();
        }

        /// <summary>
        /// Launches ball in random direction away from pitcher
        /// </summary>
        public void LaunchBall()
        {
            float x_axis_velocity = Random.Range(-3 * speed / 4, 3 * speed / 4);
            float y_axis_velocity = Mathf.Sqrt(speed * speed - x_axis_velocity * x_axis_velocity) * _mc.Pitcher.LaunchDirection;
            Rb.velocity = new Vector3(x_axis_velocity, y_axis_velocity);
        }

        public void MoveBall(Vector3 position)
        {
            gameObject.transform.position = position;
        }

        public void StopBall()
        {
            Rb.velocity = Vector3.zero;
        }


        #endregion

        public void ChangeMaterial(string material_key)
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
