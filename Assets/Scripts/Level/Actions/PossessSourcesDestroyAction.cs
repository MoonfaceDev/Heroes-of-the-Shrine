using UnityEngine;

public class PossessSourcesDestroyAction : BaseComponent
{
    public void Invoke()
    {
        var sources = FindObjectsOfType<PossessSource>();
        foreach (var source in sources)
        {
            Destroy(source.gameObject);
        }
    }
}