using UnityEngine;

/// <summary>
/// Spawns particles
/// </summary>
[RequireComponent(typeof(GameEntity))]
public class Particles : BaseComponent
{
    /// <value>
    /// Prefab of the particles
    /// </value>
    public ParticleSystem prefab;
    
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
        var entity = GetComponent<GameEntity>();
        var clone = attachToCharacter ? Instantiate(prefab, entity.transform) : Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(entity.TransformToWorld(particlePosition));

        if (!attachToCharacter)
        {
            clone.transform.rotation = entity.WorldRotation;
        }

        clone.GetComponent<Renderer>().sortingOrder = entity.SortingOrder + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var entity = GetComponent<GameEntity>();
        Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(entity.TransformToWorld(particlePosition)), 0.1f);
    }
}