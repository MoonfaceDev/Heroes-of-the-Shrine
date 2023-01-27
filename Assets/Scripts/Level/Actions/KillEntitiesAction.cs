using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                var dieBehaviour = entity.GetComponent<DieBehaviour>();
                if (!dieBehaviour)
                {
                    Debug.LogWarning("No death behaviour!");
                }

                dieBehaviour.Kill();
            }
        }
    }

    private IEnumerable<GameEntity> GetEntitiesToDestroy()
    {
        return EntityManager.Instance.GetEntities(tags.ToArray())
            .Where(entity => !entity.tags.Intersect(excludedTags).Any());
    }
}