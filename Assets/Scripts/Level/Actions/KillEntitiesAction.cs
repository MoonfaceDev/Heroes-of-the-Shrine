using System.Collections.Generic;
using System.Linq;

public class KillEntitiesAction : BaseComponent
{
    public bool destroyImmediately;
    public Tags tags;
    public Tags excludedTags;

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
                var healthSystem = entity.GetComponent<HealthSystem>();
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