using System;
using System.Linq;
using System.Net;
using Mirror;
using Mirror.Discovery;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.Events;

namespace Networking
{
    public class DiscoveryRequest : NetworkMessage { }

    public class DiscoveryResponse : NetworkMessage
    {
        // Add properties for whatever information you want the server to return to
        // clients for them to display or consume for establishing a connection.
        public long ServerId;
        public string HostName;
        public string RoomUri;
        public int gameMode;
        public IPEndPoint EndPoint { get; set; }
    }

    [Serializable]
    public class ServerFoundUnityEvent : UnityEvent<DiscoveryResponse> { };

    public class PongNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
    {
        #region Server

        public long ServerId { get; private set; }
        public UNetTransport transport;
        public ServerFoundUnityEvent OnServerFound;

        public override void Start()
        {
            ServerId = RandomLong();

            base.Start();
        }

        /// <summary>
        /// Process the request from a client
        /// </summary>
        /// <remarks>
        /// Override if you wish to provide more information to the clients
        /// such as the name of the host player
        /// </remarks>
        /// <param name="request">Request comming from client</param>
        /// <param name="endpoint">Address of the client that sent the request</param>
        /// <returns>A message containing information about this server</returns>
        protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
        {
            try
            {
                string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                transport.ConnectAddress = ipAddress;
                return new DiscoveryResponse
                {
                    ServerId = ServerId,
                    RoomUri = ipAddress,
                    HostName = PlayerPrefs.GetString("PlayerName"),
                    gameMode = PlayerPrefs.GetInt("GameMode")
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Client

        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override DiscoveryRequest GetRequest()
        {
            return new DiscoveryRequest();
        }

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint) {
            Debug.Log($"Got response {response.gameMode}; {response.HostName}");
            response.EndPoint = endpoint;
            OnServerFound.Invoke(response);
        }
        #endregion
    }
}