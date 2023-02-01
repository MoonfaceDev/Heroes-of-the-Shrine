using UnityEngine;

/// <summary>
/// Class for spawning particles when a <see cref="IHittable"/> is hit
/// </summary>
[RequireComponent(typeof(GameEntity))]
public class HitParticles : BaseComponent
{
    /// <value>
    /// Prefab of the particles
    /// </value>
    public ParticleSystem prefab;

    /// <summary>
    /// Spawns the particles in the given position, and sets its sorting layer to be higher than both the hitting and
    /// the hit object
    /// </summary>
    /// <param name="hitPoint">Source point of the particle</param>
    /// <param name="hittable">The object hit by the attack</param>
    public void Play(Vector3 hitPoint, IHittable hittable)
    {
        var entity = GetComponent<GameEntity>();
        var clone = Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(hitPoint);
        clone.transform.rotation = entity.WorldRotation;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            entity.parent.SortingOrder,
            hittable.Character.movableEntity.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }
}