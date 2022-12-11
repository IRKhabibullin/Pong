using UnityEngine;

public enum PlayerSide
{
    First,
    Second
}

[CreateAssetMenu]
public class PlayerParams : ScriptableObject
{
    public Color color;
    public PlayerSide side;
    public Vector3 ballPosition;    // Vector3(0, 2.05f, 0) local position of the ball when player is pitching
    public bool isBot;
}