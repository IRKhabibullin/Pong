using UnityEngine;

[CreateAssetMenu(menuName = "GlobalSettings/Discovery settings")]
public class SO_DiscoverySettings : ScriptableObject
{
    public string broadcastIpAddress;
    public int broadcastPort;

    [Tooltip("How often broadcast will be sent by server")]
    public float broadcastFrequency;

    [Tooltip("How often client will listen to broadcast")]
    public int discoveryFrequency;
}