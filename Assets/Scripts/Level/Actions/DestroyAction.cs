using UnityEngine;

public class DestroyAction : BaseComponent
{
    public GameObject[] objects;
    
    public void Invoke()
    {
        foreach (var @object in objects)
        {
            Destroy(@object);
        }
    }
}