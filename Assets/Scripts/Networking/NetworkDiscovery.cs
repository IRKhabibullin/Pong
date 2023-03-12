using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkDiscovery : MonoBehaviour
{
    [SerializeField] private SO_DiscoverySettings discoverySettings;

    private bool isServer;
    private bool isActive;

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private WaitForSeconds waitForBroadcastFrequency;
    private Thread listeningThread;
    private MatchData messageToSend;
    private readonly Queue<string> messagesQueue = new();

    #region API

    /// <summary>
    /// Call when want to host a match. You become a server at this point
    /// </summary>
    public void StartBroadcast(MatchData message)
    {
        if (isActive)
        {
            Debug.LogWarning("Discovery is already running. Call Shutdown before starting broadcast");
            return;
        }
        
        isServer = true;
        messageToSend = message;

        try
        {
            udpClient = new UdpClient(discoverySettings.broadcastPort);
            udpClient.EnableBroadcast = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to listen for UDP at port {discoverySettings.broadcastPort}: {e.Message}");
            return;
        }

        endPoint = new IPEndPoint(IPAddress.Parse(discoverySettings.broadcastIpAddress),
            discoverySettings.broadcastPort);
        waitForBroadcastFrequency = new WaitForSeconds(discoverySettings.broadcastFrequency);

        isActive = true;
        StartCoroutine(BroadcastEnumerator());
    }

    /// <summary>
    /// Call when want to find a hosted match and subscribe to EventsManager.NetworkChannel.OnMatchFound event.
    /// Can not use while hosting a match
    /// </summary>
    public void StartListening()
    {
        if (isServer)
        {
            Debug.LogWarning("Can not discover broadcasts while being a server");
            return;
        }

        try
        {
            udpClient = new UdpClient(discoverySettings.broadcastPort);
            udpClient.EnableBroadcast = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to listen for UDP at port {discoverySettings.broadcastPort}: {e.Message}");
            return;
        }

        endPoint = new IPEndPoint(IPAddress.Any, 0);

        isActive = true;
        listeningThread = new Thread(ListenForMessages)
        {
            IsBackground = true
        };
        listeningThread.Start();

        StartCoroutine(ListenBroadcastEnumerator());
    }

    public void Shutdown()
    {
        isActive = false;

        if (udpClient != null)
        {
            if (!isServer)
            {
                listeningThread.Abort();
            }

            try
            {
                udpClient.Close();
                udpClient.Dispose();
            }
            catch (Exception)
            {
                // it is just close, swallow the error
            }

            udpClient = null;
        }

        isServer = false;
    }

    public static IPAddress GetCurrentIP()
    {
        return (
            from netInterface in NetworkInterface.GetAllNetworkInterfaces()
            where
                netInterface.OperationalStatus == OperationalStatus.Up &&
                netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet
            from addressInfo in netInterface.GetIPProperties().UnicastAddresses
            where addressInfo.Address.AddressFamily == AddressFamily.InterNetwork
            select addressInfo.Address
        ).FirstOrDefault();
    }

    #endregion

    #region Logic
    
    private void ListenForMessages()
    {
        while (isActive)
        {
            try
            {
                Debug.Log(
                    $"Starting listening on {discoverySettings.broadcastIpAddress}:{discoverySettings.broadcastPort}");

                var receiveBytes = udpClient.Receive(ref endPoint);
                var returnData = Encoding.UTF8.GetString(receiveBytes);

                lock (messagesQueue)
                {
                    messagesQueue.Enqueue(returnData);
                }
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004)
                    Debug.Log($"Socket exception while receiving data from udp client: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving data from udp client: {e.Message}");
            }

            Thread.Sleep(discoverySettings.discoveryFrequency);
        }
    }

    private IEnumerator ListenBroadcastEnumerator()
    {
        while (true)
        {
            foreach (var message in GetMessages())
            {
                EventsManager.Instance.NetworkChannel.RaiseOnMatchFoundEvent(message);
            }

            yield return discoverySettings.discoveryFrequency;
        }
    }

    private IEnumerable<MatchData> GetMessages()
    {
        var messages = new List<MatchData>();
        lock (messagesQueue)
        {
            while (messagesQueue.Count > 0)
            {
                messages.Add(JsonUtility.FromJson<MatchData>(messagesQueue.Dequeue()));
            }
        }

        return messages;
    }

    private IEnumerator BroadcastEnumerator()
    {
        while (isActive)
        {
            Send(messageToSend);

            yield return waitForBroadcastFrequency;
        }
    }

    private void Send(MatchData message)
    {
        Debug.Log("Broadcasting message");
        var json = JsonUtility.ToJson(message);
        var sendBytes = Encoding.UTF8.GetBytes(json);
        udpClient.Send(sendBytes, sendBytes.Length, endPoint);
    }

    #endregion

    private void OnDisable()
    {
        Shutdown();
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    private void OnApplicationQuit()
    {
        Shutdown();
    }

    #region singleton setup
    public static NetworkDiscovery Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkDiscovery>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<NetworkDiscovery>();
                }
            }
            return _instance;
        }
    }

    public static bool HasInstance => _instance != null;
    
    private static NetworkDiscovery _instance;

    void Awake()
    {
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    #endregion
}