using System;
using UnityEngine;

namespace Singleplayer
{
    public class PlatformController : MonoBehaviour, IPlatformController
    {
        private GameController _gc;

        private readonly float width = 5f;
        private readonly float maxSpeed = 30f;
        private Vector3 ballPosition = new Vector3(0, 2.05f, 0);  // where ball must be placed when player is pitcher

        public string Name;
        public int LaunchDirection { get; set; }
        public Vector3 mSpeed = Vector3.zero;  // current speed. Used only on server. Position on client is synced from server
        public float mAngle = 0;  // current rotation speed. Used only on server. Rotation on client is synced from server
        public Vector3 mPosition = new Vector3();  // synced variable for platform position
        public Quaternion mRotation = new Quaternion();  // synced variable for platform rotation

        private void Awake()
        {
            _gc = GameObject.Find("GameManager").GetComponent<GameController>();
            Name = PlayerPrefs.GetString("PlayerName");
        }

        void FixedUpdate()
        {
            if (_gc.matchController.MovementAllowed() || _gc.debugMode)
            {
                // move
                if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(mSpeed.x), 0, 0), width, LayerMask.GetMask("SideWall")))
                    mPosition += mSpeed * Time.fixedDeltaTime;
            }
            // sync position on clients
            transform.position = mPosition;
            transform.rotation = mRotation;
        }

        public void SetUp(int side)
        {
            LaunchDirection = side == 0 ? 1 : -1;
            mPosition = _gc.playersPositions[side].position;
        }

        public void SetColor(int playerNumber)
        {
            GetComponent<MeshRenderer>().material = _gc.playersMaterials[playerNumber];
        }

        public void SetRotation(float rotation)
        {
            mRotation = Quaternion.Euler(Vector3.forward * rotation);
        }

        public void SetSpeed(float direction)
        {
            if (Math.Abs(direction) < 0.1)
            {
                mSpeed = Vector3.zero;
                return;
            }
            mSpeed = new Vector3(direction, 0, 0) * maxSpeed;
        }

        public float GetCurrentAngle()
        {
            return ((transform.localRotation.eulerAngles.z + 180f) % 360f) - 180f;
        }

        public Vector2 GetBallStartPosition()
        {
            return new Vector3(mPosition.x, mPosition.y, 0) + ballPosition * LaunchDirection;
        }

        public void ResetPlatform()
        {
            mSpeed = Vector3.zero;
            mAngle = 0;
            mPosition = new Vector3(0f, mPosition.y, 0f);
            mRotation = Quaternion.identity;
        }
    }
}
