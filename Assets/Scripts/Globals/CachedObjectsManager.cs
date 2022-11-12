using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class CachedObjectsManager : MonoBehaviour
{
    public string[] cacheTags;

    public static CachedObjectsManager Instance { get; private set; }
    private Dictionary<string, List<Object>> objects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        objects = new Dictionary<string, List<Object>>();
        foreach (var cacheTag in cacheTags)
        {
            objects[cacheTag] = new List<Object>();
        }
    }

    private List<Object> SafeGet(string cacheTag)
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

    public void AddObject(string cacheTag, Object @object)
    {
        SafeGet(cacheTag).Add(@object);
    }

    public void RemoveObject(string cacheTag, Object @object)
    {
        SafeGet(cacheTag).Remove(@object);
    }

    public T[] GetObjects<T>(string cacheTag) where T : Object
    {
        return SafeGet(cacheTag).Select(@object => (T) @object).ToArray();
    }

    public T GetObject<T>(string cacheTag) where T : Object
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
