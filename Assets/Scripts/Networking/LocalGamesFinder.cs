using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalGamesFinder : MonoBehaviour
{
    private List<ServerInfoObject> discoveredServerInfoObjects = new();
    
    private WaitForSeconds listenPeriod = new(1); 

    public string BroadcastIpAddress = "255.255.255.255";
    public ushort BroadcastPort = 8014;

    private UdpConnection connection;

    private void Start()
    {
        connection = new UdpConnection();
        connection.StartConnection(BroadcastIpAddress, BroadcastPort);
        connection.StartListening();

        StartCoroutine(ListenBroadcastEnumerator());
    }

    private IEnumerator ListenBroadcastEnumerator()
    {
        while (true)
        {
            foreach (var serverInfo in connection.GetMessages())
            {
                ReceivedServerInfo(serverInfo);
            }

            yield return listenPeriod;
        }
    }

    private void ReceivedServerInfo(ServerInfoObject serverInfo)
    {
        var ipExists = false;

        foreach (var discoveredInfo in discoveredServerInfoObjects.Where(discoveredInfo => serverInfo.ipAddress == discoveredInfo.ipAddress))
        {
            ipExists = true;
            var receivedTime = float.Parse(serverInfo.timeStamp);
            var storedTime = float.Parse(discoveredInfo.timeStamp);

            if (receivedTime <= storedTime)
                continue;
            
            discoveredInfo.gameName = serverInfo.gameName;
            discoveredInfo.timeStamp = serverInfo.timeStamp;
        }
        if (!ipExists)
        {
            discoveredServerInfoObjects.Add(serverInfo);
        }
    }

    private void OnDestroy()
    {
        connection.Stop();
    }
}