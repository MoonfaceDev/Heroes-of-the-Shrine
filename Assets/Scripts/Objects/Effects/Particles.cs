using UnityEngine;

/// <summary>
/// Spawns particles
/// </summary>
public class Particles : EntityBehaviour
{
    /// <value>
    /// Prefab of the particles
    /// </value>
    public GameObject prefab;
    
    /// <value>
    /// Point where particle is spawned, relative to the <see cref="GameEntity"/>
    /// </value>
    public Vector3 particlePosition;
    
    /// <value>
    /// If <c>true</c>, particles move along with the character after they are spawned
    /// </value>
    public bool attachToCharacter;

    /// <summary>
    /// Spawns the particles
    /// </summary>
    public void Play()
    {
        var clone = attachToCharacter ? Instantiate(prefab, Entity.transform) : Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(Entity.TransformToWorld(particlePosition));

        if (!attachToCharacter)
        {
            clone.transform.rotation = Entity.WorldRotation;
        }

        clone.GetComponent<Renderer>().sortingOrder = Entity.SortingOrder + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(Entity.TransformToWorld(particlePosition)), 0.1f);
    }
}