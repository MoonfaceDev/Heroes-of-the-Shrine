using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CachedObjectsManager : MonoBehaviour
{
    public string[] cacheTags;

    public static CachedObjectsManager Instance { get; private set; }
    private Dictionary<string, List<UnityEngine.Object>> objects;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        objects = new();
        foreach (string cacheTag in cacheTags)
        {
            objects[cacheTag] = new();
        }
    }

    private List<UnityEngine.Object> SafeGet(string cacheTag)
    {
        try
        {
            return objects[cacheTag];
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException($"The given cache tag '{cacheTag}' is not present in CachedObjectsManager");
        }
    }

    public void AddObject(string cacheTag, UnityEngine.Object @object)
    {
        SafeGet(cacheTag).Add(@object);
    }

    public void RemoveObject(string cacheTag, UnityEngine.Object @object)
    {
        SafeGet(cacheTag).Remove(@object);
    }

    public T[] GetObjects<T>(string cacheTag) where T : UnityEngine.Object
    {
        return SafeGet(cacheTag).Select(@object => (T) @object).ToArray();
    }

    public T GetObject<T>(string cacheTag) where T : UnityEngine.Object
    {
        try
        {
            return SafeGet(cacheTag)[0] as T;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}
