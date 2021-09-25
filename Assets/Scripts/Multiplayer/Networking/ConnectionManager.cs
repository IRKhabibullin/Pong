using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiplayer
{
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private NetworkMatchController matchController;
        [SerializeField] private GameObject serverDisconnectedPanel;
        [SerializeField] private PongNetworkDiscovery discovery;
        [SerializeField] private Button leaveButton;

        public UnityEvent onLeave;

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        /// <summary>
        /// This callback is invoked on a client when the server shuts down and invoked on a server when other client disconnects.
        /// </summary>
        private void HandleClientDisconnect(ulong ClientId)
        {
            // Case when server disconnected and we are client
            if (ClientId == NetworkManager.Singleton.LocalClientId)
            {
                serverDisconnectedPanel.SetActive(true);
                gameController.QuitToMenu();
            }
            // Case when other client disconnected and we are host
            if (NetworkManager.Singleton.IsHost)
            {
                matchController.ExitMatch();
            }
        }

        /// <summary>
        /// runs on the server and on the local client that connects
        /// </summary>
        private void HandleClientConnected(ulong ClientId)
        {
            leaveButton.onClick.AddListener(Leave);
            if (ClientId == NetworkManager.Singleton.LocalClientId)
            {
                matchController = gameController.matchController as NetworkMatchController;
                matchController.EnterMatch();
            }
            if (NetworkManager.Singleton.IsServer)
            {
                int sideId = 0;
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    if (client.ClientId == ClientId)
                    {
                        client.PlayerObject.gameObject.AddComponent<PlatformController>();
                        client.PlayerObject.tag = $"Player{sideId + 1}";
                        client.PlayerObject.GetComponent<PlatformController>().SetUp(sideId);
                        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
                        {
                            client.PlayerObject.GetComponent<PlatformController>().IsLeader.Value = true;
                        }
                        break;
                    }
                    sideId++;
                }
                if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
                {
                    discovery.StopDiscovery();
                    matchController.OnBothPlayersConnected();
                }
            }
        }

        /// <summary>
        /// Calling here OnClientConnectedCallback because host is server and client at the same time
        /// </summary>
        private void HandleServerStarted()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                HandleClientConnected(NetworkManager.Singleton.LocalClientId);
            }
        }

        public void Host()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void Find()
        {
            discovery.StartDiscovery();
        }

        public void Join(string hostAddress)
        {
            discovery.StopDiscovery();
            discovery.transport.ConnectAddress = hostAddress;
            NetworkManager.Singleton.StartClient();
        }

        public void Leave()
        {
            onLeave.Invoke();

            if (NetworkManager.Singleton.IsServer)
            {
                // if server is leaving, disconnect another player
                foreach (var playerClientId in NetworkManager.Singleton.ConnectedClients.Keys)
                {
                    if (playerClientId != NetworkManager.Singleton.LocalClientId)
                    {
                        NetworkManager.Singleton.DisconnectClient(playerClientId);
                        break;
                    }
                }
                NetworkManager.Singleton.StopHost();
            }
            else
            {
                NetworkManager.Singleton.StopClient();
            }
        }
    }
}
