using System;
using System.Net;
/*using Mirror;
using Mirror.Discovery;*/
using UnityEngine;
using UnityEngine.Events;

namespace Networking
{
    /*public class DiscoveryRequest : MessageBase { }

    public class DiscoveryResponse : MessageBase
    {
        // Add properties for whatever information you want the server to return to
        // clients for them to display or consume for establishing a connection.
        public long ServerId;
        public string HostName;
        public int PlayersInRoom;
        public Uri RoomUri;
        public IPEndPoint EndPoint { get; set; }
    }

    [Serializable]
    public class ServerFoundUnityEvent : UnityEvent<DiscoveryResponse> { };

    public class PongNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
    {
        #region Server

        public long ServerId { get; private set; }
        public Transport transport;
        public ServerFoundUnityEvent OnServerFound;

        public override void Start()
        {
            ServerId = RandomLong();

            // active transport gets initialized in awake
            // so make sure we set it here in Start()  (after awakes)
            // Or just let the user assign it in the inspector
            if (transport == null)
                transport = Transport.activeTransport;

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
                Debug.Log("Entered");
                Debug.Log($"Processing client request: {request}");
                //string hostName = GameObject.Find("NameText").GetComponent<TextMeshProUGUI>().text;
                int playersCount = GameObject.Find("NetworkManager").GetComponent<PongNetworkManager>().numPlayers;
                string hostName = "qwerfv";
                Debug.Log($"playersCount {playersCount}; hostName: {hostName}");
                return new DiscoveryResponse
                {
                    ServerId = ServerId,
                    RoomUri = transport.ServerUri(),
                    HostName = hostName,
                    PlayersInRoom = playersCount
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
            catch (Exception e)
            {
                Debug.Log($"Catched {e}");
                throw;
            }
            finally
            {
                Debug.Log("Finnaly");
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
            Console.WriteLine("Sending client request");
            //Debug.Log("Sending client request");
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

            Debug.Log($"Processing server response: {response}");
            response.EndPoint = endpoint;

            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.RoomUri)
            {
                Host = response.EndPoint.Address.ToString()
            };
            response.RoomUri = realUri.Uri;
            Debug.Log("Processing of server response finished. Invoking events handlers...");

            OnServerFound.Invoke(response);
        }
        #endregion
    }*/
}