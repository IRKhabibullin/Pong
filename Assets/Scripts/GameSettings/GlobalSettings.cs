using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] private SO_GlobalSettings globalSettings;
    [SerializeField] private SO_PlatformSettings platformSettings;
    [SerializeField] private SO_BallSettings ballSettings;

    public static SO_GlobalSettings Settings => Instance.globalSettings;
    public static SO_PlatformSettings Platform => Instance.platformSettings;
    public static SO_BallSettings Ball => Instance.ballSettings;

    #region singleton setup
    public static GlobalSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalSettings>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<GlobalSettings>();
                }
            }
            return _instance;
        }
    }

    public static bool HasInstance => _instance != null;
    
    private static GlobalSettings _instance;

    void Awake()
    {
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    #endregion

    public static BoardSide GetSideByTag(string tag)
    {
        return tag switch
        {
            "Blue" => BoardSide.Blue,
            "Red" => BoardSide.Red,
            _ => BoardSide.Red
        };
    }

    public static bool LayerIncluded(LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}
