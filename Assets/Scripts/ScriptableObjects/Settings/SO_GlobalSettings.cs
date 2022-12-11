using UnityEngine;

[CreateAssetMenu(menuName = "GlobalSettings/Global settings")]
public class SO_GlobalSettings : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject platformPrefab;
    public GameObject ballPrefab;

    [Header("Layers")]
    public LayerMask BackWallLayer;
    public LayerMask SideWallLayer;
    public LayerMask PlatformLayer;

    [Header("Input system")]
    public string HorizontalAxis;
}
