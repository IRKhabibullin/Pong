using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Mirror;
using Mirror.Discovery;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class DiscoveryRequest : NetworkMessage { }

    public class DiscoveryResponse : NetworkMessage
    {
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
        public RoomListController roomListController;

        public override void Start()
        {
            ServerId = RandomLong();

            base.Start();
        }

        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
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
                string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                    f => f.AddressFamily == AddressFamily.InterNetwork).ToString();
                transport.ConnectAddress = ipAddress;
                GameObject.Find("GameManager").GetComponent<GameController>().SetDebugText(ipAddress);
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
            IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                    f => f.AddressFamily == AddressFamily.InterNetwork);
            Debug.Log($"Ip {ipAddress} {GetSubnetMask(ipAddress)}");
            transport.ConnectAddress = ipAddress.ToString();
            GameObject.Find("GameManager").GetComponent<GameController>().SetDebugText($"{ipAddress} {GetSubnetMask(ipAddress)}");
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
        protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
        {
            response.EndPoint = endpoint;
            OnServerFound.Invoke(response);
        }
        #endregion
    }
}