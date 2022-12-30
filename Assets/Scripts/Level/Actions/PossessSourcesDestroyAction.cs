using UnityEngine;

public class PossessSourcesDestroyAction : MonoBehaviour
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
