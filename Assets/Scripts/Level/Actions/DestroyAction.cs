using UnityEngine;

/// <summary>
/// Destroys multiple <see cref="GameObject"/>
/// </summary>
public class DestroyAction : BaseComponent
{
    /// <summary>
    /// Objects to destroy
    /// </summary>
    public GameObject[] objects;
    
    /// <summary>
    /// Destroy <see cref="objects"/>
    /// </summary>
    public void Invoke()
    {
        foreach (var @object in objects)
        {
            Destroy(@object);
        }
    }
}