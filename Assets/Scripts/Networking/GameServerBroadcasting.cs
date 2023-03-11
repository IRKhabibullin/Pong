using System;
using UnityEngine;
using System.Collections;
 
[Serializable]
public class ClientServerInfo
{
    public bool IsServer = false;
    public bool IsClient = false;
    public string ConnectToServerIp;
    public ushort GamePort = 5001;

    public string GameName;

    public string BroadcastIpAddress;
    public ushort BroadcastPort;

}

public class GameServerBroadcasting : MonoBehaviour
{
    private UdpConnection connection;

    public float broadcastPeriod = 2f;
    private float nextTime;
    private WaitForSeconds broadcastWaitForSeconds;

    [SerializeField] private ClientServerInfo clientServerInfo;

    private void Start()
    {
        if (!clientServerInfo.IsServer)
        {
            enabled = false;
        }

        var sendToIp = clientServerInfo.BroadcastIpAddress;
        int sendToPort = clientServerInfo.BroadcastPort;

        broadcastWaitForSeconds = new WaitForSeconds(broadcastPeriod);

        connection = new UdpConnection();
        Debug.Log($"Starting broadcasting on {sendToIp}:{sendToPort}");
        connection.StartConnection(sendToIp, sendToPort);

        StartCoroutine(BroadcastEnumerator());
    }

    private IEnumerator BroadcastEnumerator()
    {
        while (true)
        {
            connection.Send(nextTime, clientServerInfo.GameName);
            nextTime += broadcastPeriod;

            yield return broadcastWaitForSeconds;
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"Stopping broadcasting on {clientServerInfo.BroadcastIpAddress}:{clientServerInfo.BroadcastPort}");
        connection.Stop();
    }
}