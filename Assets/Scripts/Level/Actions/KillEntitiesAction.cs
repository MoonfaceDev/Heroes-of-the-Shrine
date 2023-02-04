using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Kills all entities that match specified tags, using their <see cref="DieBehaviour"/>
/// </summary>
public class KillEntitiesAction : BaseComponent
{
    /// <value>
    /// If <c>true</c>, entities will destroy immediately, without calling <see cref="DieBehaviour"/> 
    /// </value>
    public bool destroyImmediately;
    
    /// <value>
    /// Tags of the entities to destroy
    /// </value>
    public Tags tags;
    
    /// <value>
    /// Tags of excluded entities that are not destroyed - even if the entity matches <see cref="tags"/>
    /// </value>
    public Tags excludedTags;

    /// <summary>
    /// Kills the matching entities
    /// </summary>
    public void Invoke()
    {
        var entities = GetEntitiesToDestroy();

        foreach (var entity in entities)
        {
            if (destroyImmediately)
            {
                Destroy(entity.gameObject);
            }
            else
            {
                var healthSystem = entity.GetBehaviour<HealthSystem>();
                healthSystem.Kill();
            }
        }
    }

    private IEnumerable<GameEntity> GetEntitiesToDestroy()
    {
        return EntityManager.Instance.GetEntities(tags.ToArray())
            .Where(entity => !entity.tags.Intersect(excludedTags).Any());
    }
}