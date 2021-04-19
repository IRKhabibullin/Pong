using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    public class PongManager : NetworkManager
    {
        [SerializeField] private GameController gameController;

        /*public Vector3 firstPlayerPosition;
        public Vector3 secondPlayerPosition;
        public Vector3 firstPlayerRotation;
        public Vector3 secondPlayerRotation;
        private GameObject ball;
        
        *//*[SerializeField]
        private PongNetworkDiscovery networkDiscovery;*//*

        public List<PlayerController> players { get; } = new List<PlayerController>();*/

        /*public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Vector3 startPosition = numPlayers == 0 ? firstPlayerPosition : secondPlayerPosition;
            Vector3 startRotation = numPlayers == 0 ? firstPlayerRotation : secondPlayerRotation;
            GameObject player = Instantiate(playerPrefab, startPosition, Quaternion.Euler(startRotation));
            player.GetComponent<PlayerController>()._name = $"player{players.Count}";
            NetworkServer.AddPlayerForConnection(conn, player);
            //players.Add(conn.identity.GetComponent<PlayerController>());

            // spawn ball if two players
            if (numPlayers == 2)
            {
                networkDiscovery.StopDiscovery();
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                NetworkServer.Spawn(ball);
                gameController.InitGame();
            }
        }*/

        /*public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<PlayerController>();
                players.Remove(player);
                
                NotifyPlayersOfReadyState();
            }

            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }

        public void NotifyPlayersOfReadyState()
        {

        }
        
        public new void StartHost()
        {
            base.StartHost();
            PlayerPrefs.SetInt("PlayerMode", (int)GameController.PlayerMode.Multiplayer);
            gameController.EnterGame();
            networkDiscovery.AdvertiseServer();
        }
        
        public void StopNetworking()
        {
            switch (mode)
            {
                case NetworkManagerMode.ClientOnly:
                    StopClient();
                    break;
                case NetworkManagerMode.Host:
                    StopHost();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Not supposed to exit to menu in mode {mode}");
            }
        }

        public override void OnStopServer()
        {
            players.Clear();
            base.OnStopServer();
        }*/
    }
}
