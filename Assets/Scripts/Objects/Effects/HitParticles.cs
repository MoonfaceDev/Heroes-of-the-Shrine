using UnityEngine;

/// <summary>
/// Class for spawning particles when a <see cref="IHittable"/> is hit
/// </summary>
public class HitParticles : EntityBehaviour
{
    /// <value>
    /// Prefab of the particles
    /// </value>
    public GameObject prefab;

    /// <summary>
    /// Spawns the particles in the given position, and sets its sorting layer to be higher than both the hitting and
    /// the hit object
    /// </summary>
    /// <param name="hitPoint">Source point of the particle</param>
    /// <param name="hittable">The object hit by the attack</param>
    public void Play(Vector3 hitPoint, IHittable hittable)
    {
        var clone = Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(hitPoint);
        clone.transform.rotation = Entity.WorldRotation;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            Entity.parent.SortingOrder,
            hittable.Character.movableEntity.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }
}