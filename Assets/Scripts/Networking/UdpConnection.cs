using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

public class UdpConnection
{
    private UdpClient udpClient;

    private string sendToIp;
    private int sendOrReceivePort;

    private readonly Queue<string> incomingQueue = new();
    Thread receiveThread;

    private bool threadRunning;

    //The server will need to find its IP address so it can send it out to clients
    private IPAddress serverIp;

    public void StartConnection(string sendToIp, int sendOrReceivePort)
    {
        try
        {
            udpClient = new UdpClient(sendOrReceivePort);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to listen for UDP at port " + sendOrReceivePort + ": " + e.Message);
            return;
        }

        serverIp = GetCurrentIP();

        udpClient.EnableBroadcast = true;

        this.sendToIp = sendToIp;
        this.sendOrReceivePort = sendOrReceivePort;
    }

    private static IPAddress GetCurrentIP()
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

    //This will only be called by the client in order to start "listening"
    public void StartListening()
    {
        receiveThread = new Thread(() => ListenForMessages(udpClient))
        {
            IsBackground = true
        };
        threadRunning = true;
        receiveThread.Start();
    }

    private void ListenForMessages(UdpClient client)
    {
        var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (threadRunning)
        {
            try
            {
                Debug.Log($"starting receive on {serverIp} and port {sendOrReceivePort}");

                var receiveBytes = client.Receive(ref remoteIpEndPoint);
                var returnData = Encoding.UTF8.GetString(receiveBytes);
 
                lock (incomingQueue)
                {
                    incomingQueue.Enqueue(returnData);
                }
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004)
                    Debug.Log("Socket exception while receiving data from udp client: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving data from udp client: " + e.Message);
            }
            Thread.Sleep(1);
        }
    }

    //This is another method the client will call to grab all the messages that have been received by listening
    public IEnumerable<ServerInfoObject> GetMessages()
    {
        lock (incomingQueue)
        {
            return incomingQueue.Select(JsonUtility.FromJson<ServerInfoObject>);
        }
    }

    //We will only call this method on the server and provide it the game name and time
    //We don't provide the game name in StartConnection because the client won't have it and 
    //both the client and server use StartConnection
    public void Send(float floatTime, string gameName)
    {
        var stringTime = floatTime.ToString(CultureInfo.InvariantCulture);

        var sendToEndpoint = new IPEndPoint(IPAddress.Parse(sendToIp), sendOrReceivePort);

        var thisServerInfoObject = new ServerInfoObject
        {
            gameName = gameName,
            ipAddress = serverIp.ToString(),
            timeStamp = stringTime
        };

        var json = JsonUtility.ToJson(thisServerInfoObject);
        var sendBytes = Encoding.UTF8.GetBytes(json);
        udpClient.Send(sendBytes, sendBytes.Length, sendToEndpoint);
    }

    public void Stop()
    {
        if (threadRunning)
        {
            threadRunning = false;
            receiveThread.Abort();
        }

        udpClient.Close();
        udpClient.Dispose();
    }
}

[Serializable]
public class ServerInfoObject
{
    public string gameName = "";
    public string ipAddress = "";
    public string timeStamp = "";
}