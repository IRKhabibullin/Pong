using System;
using UnityEngine;

namespace Singleplayer
{
    public class PlatformController : MonoBehaviour, IPlatformController
    {
        private GameController _gc;

        private readonly float width = 5f;
        private readonly float maxSpeed = 40f;
        private Vector3 ballPosition = new Vector3(0, 2.05f, 0);  // where ball must be placed when player is pitcher

        public string Name;
        public int LaunchDirection { get; set; }
        public Vector3 mSpeed = Vector3.zero;  // current speed
        public float mAngle = 0;  // current rotation speed
        public Vector3 mPosition = new Vector3();  // platform position
        public Quaternion mRotation = new Quaternion();  // platform rotation
        public float maxAngle = 45f;
        public float difficultySpeedRatio = 1f;

        private void Awake()
        {
            _gc = GameObject.Find("GameManager").GetComponent<GameController>();
            Name = PlayerPrefs.GetString("PlayerName");
            SetSpeedRatio(PlayerPrefs.GetString("Difficulty", "normal"));
        }

        void FixedUpdate()
        {
            if (_gc.debugMode || _gc.testPlatform != null || _gc.matchController.MovementAllowed())
            {
                // move
                if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(mSpeed.x), 0, 0), width, LayerMask.GetMask("SideWall")))
                    mPosition += mSpeed * Time.fixedDeltaTime;
            }
            // sync position on clients
            transform.position = mPosition;
            transform.rotation = mRotation;
        }

        public void SetSpeedRatio(string difficulty)
        {
            switch (difficulty)
            {
                case "easy":
                    difficultySpeedRatio = 1.5f;
                    break;
                case "normal":
                    difficultySpeedRatio = 1f;
                    break;
                case "hard":
                    difficultySpeedRatio = 0.7f;
                    break;
            }
            if (gameObject.GetComponent<AIController>())
            {
                difficultySpeedRatio = 1 / difficultySpeedRatio;
            }
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
            mSpeed = new Vector3(direction, 0, 0) * maxSpeed * difficultySpeedRatio;
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
