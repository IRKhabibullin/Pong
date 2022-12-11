using UnityEngine;

[CreateAssetMenu(menuName = "GlobalSettings/Platform settings")]
public class SO_PlatformSettings : ScriptableObject
{
    public float Width;
    public float MaxAngle;
    public float MaxMoveSpeed;
    public float MaxRotationSpeed;
}
