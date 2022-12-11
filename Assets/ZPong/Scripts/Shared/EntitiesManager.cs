using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager to store all entities across scenes. MonoBehaviours can access them via their game objects instanceIDs
/// </summary>
public class EntitiesManager : MonoBehaviour
{
    #region singleton setup
    public static EntitiesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EntitiesManager>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<EntitiesManager>();
                }
            }
            return _instance;
        }
    }
    private static EntitiesManager _instance;

    void Awake()
    {
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    #endregion

    // keys are game object instanceIDs
    private static Dictionary<int, IEntity> entities;

    public static void AddEntity<T>(GameObject _go, T entityType) where T : IEntity
    {
        // todo probably need to pass type of entity as well. Manager should create an instance of entity itself
        entities.Add(_go.GetInstanceID(), entityType);
    }

    public IEntity GetEntity(GameObject _go)
    {
        if (!entities.ContainsKey(GetGameObjectIdentifier(_go)))
        {
            // todo need to decide how to determine type of IEntity that must be created here
            // AddEntity(_go, new Platform());
        }
        return entities[GetGameObjectIdentifier(_go)];
    }

    public static void OnObjectDestroyed(GameObject _go)
    {
        // todo need to figure out how to remove entities for deleted GameObjects
        entities.Remove(_go.GetInstanceID());
    }

    private int GetGameObjectIdentifier(GameObject _go)
    {
        return _go.GetInstanceID();
    }
}
