using System;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer
{
    public class PlatformController : NetworkBehaviour, IPlatformController
    {
        //public NetworkVariable<string> Name = new NetworkVariable<string>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly });
        public NetworkVariable<bool> IsReady = new NetworkVariable<bool>();
        public NetworkVariable<bool> IsLeader = new NetworkVariable<bool>();

        private readonly float width = 5f;
        private readonly float maxSpeed = 40f;
        private Vector3 ballPosition = new Vector3(0, 2.05f, 0);  // where ball must be placed when player is pitcher

        public int LaunchDirection { get; set; }
        public Vector3 mSpeed = Vector3.zero;  // current speed. Used only on server. Position on client is synced from server
        public float mAngle = 0;  // current rotation speed. Used only on server. Rotation on client is synced from server

        private GameControllerOld _gc;

        public NetworkVariable<Vector3> mPosition = new NetworkVariable<Vector3>();  // synced variable for platform position
        public NetworkVariable<Quaternion> mRotation = new NetworkVariable<Quaternion>();  // synced variable for platform rotation

        private void Awake()
        {
            _gc = GameObject.Find("GameManager").GetComponent<GameControllerOld>();
            if (IsOwner)
            {
                //Name.Value = PlayerPrefs.GetString("PlayerName");
            }
        }

        void FixedUpdate()
        {
            if (_gc.matchController.MovementAllowed() || _gc.debugMode)
            {
                // move
                if (!Physics.Raycast(transform.position, new Vector3(Math.Sign(mSpeed.x), 0, 0), width, LayerMask.GetMask("SideWall")))
                    mPosition.Value += mSpeed * Time.fixedDeltaTime;
            }
            // sync position on clients
            transform.position = mPosition.Value;
            transform.rotation = mRotation.Value;
        }

        public void SetUp(int side)
        {
            LaunchDirection = side == 0 ? 1 : -1;
            mPosition.Value = _gc.playersPositions[side].position;
            SetColorClientRpc(side);
        }

        public void SetColor(int playerNumber)
        {
            GetComponent<MeshRenderer>().material = _gc.playersMaterials[playerNumber];
        }

        [ServerRpc]
        public void SetRotationServerRpc(float rotation)
        {
            SetRotation(rotation);
        }

        public void SetRotation(float rotation)
        {
            mRotation.Value = Quaternion.Euler(Vector3.forward * rotation);
        }

        [ServerRpc]
        public void SetSpeedServerRpc(float direction)
        {
            SetSpeed(direction);
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

        /// <summary>
        /// Returns where the ball should be placed on a platform at start of the round
        /// </summary>
        public Vector2 GetBallStartPosition()
        {
            return new Vector3(mPosition.Value.x, mPosition.Value.y, 0) + ballPosition * LaunchDirection;
        }

        [ClientRpc]
        public void SetColorClientRpc(int playerNumber)
        {
            SetColor(playerNumber);
        }

        /// <summary>
        /// Resets position and rotation of platform. Called only on server
        /// </summary>
        public void ResetPlatform()
        {
            mSpeed = Vector3.zero;
            mAngle = 0;
            mPosition.Value = new Vector3(0f, mPosition.Value.y, 0f);
            mRotation.Value = Quaternion.identity;
        }

        [ServerRpc]
        public void TogglePlayerReadyServerRpc()
        {
            IsReady.Value = !IsReady.Value;
            bool everyoneIsReady = true;
            // if everyone except host is ready, then host can start a round
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.ClientId != NetworkManager.Singleton.LocalClientId && !client.PlayerObject.GetComponent<PlatformController>().IsReady.Value)
                {
                    everyoneIsReady = false;
                    break;
                }
            }
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                (_gc.matchController as NetworkMatchController).ReadyToStart(everyoneIsReady);
            }
        }
    }
}
