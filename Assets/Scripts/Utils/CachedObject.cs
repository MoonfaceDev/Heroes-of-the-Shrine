using UnityEngine;

public class CachedObject : MonoBehaviour
{
    public string cacheTag;
    public Object @object;

    private void Awake()
    {
        CachedObjectsManager.Instance.AddObject(cacheTag, @object);
    }

    private void OnDestroy()
    {
        CachedObjectsManager.Instance.RemoveObject(cacheTag, @object);
    }
}