using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ParticlesHitExecutor : IHitExecutor
{
    /// <value>
    /// Prefab of the particles
    /// </value>
    public GameObject prefab;

    public void Execute(Hit hit)
    {
        var clone = Object.Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(hit.Point);
        clone.transform.rotation = hit.Direction;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            hit.Source.Entity.SortingOrder,
            hit.Victim.RelatedEntity.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }
}