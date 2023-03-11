using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    #region singleton setup
    public static ConnectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ConnectionManager>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<ConnectionManager>();
                }
            }
            return _instance;
        }
    }
    
    private static ConnectionManager _instance;

    void Awake()
    {
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    #endregion

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Join(MatchData matchData)
    {
        // discovery.StopDiscovery();
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            matchData.ipAddress,
            (ushort)matchData.port
        );
        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
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
        }
        NetworkManager.Singleton.Shutdown();
    }

    public static IPAddress GetLocalIPAddress()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == AddressFamily.InterNetwork);
    }
    
    public static IPAddress GetSubnetMask()
    {
        var address = GetLocalIPAddress();
        
        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (var unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
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
        throw new ArgumentException($"Can't find subnetmask for IP address '{address}'");
    }
}
