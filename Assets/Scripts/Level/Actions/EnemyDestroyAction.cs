using System.Linq;
using UnityEngine;

public class EnemyDestroyAction : BaseComponent
{
    public Tags tags;
    public Tags excludedTags;

    public void Invoke()
    {
        var enemies = EntityManager.Instance.GetEntities(Tag.Enemy);
        foreach (var enemy in enemies)
        {
            DestroyIfIncluded(enemy);
        }
    }

    private void DestroyIfIncluded(GameEntity entity)
    {
        if (entity.tags.Intersect(excludedTags).Any()) return;
        if (!entity.tags.Intersect(tags).Any()) return;

        var dieBehaviour = entity.GetComponent<DieBehaviour>();
        if (!dieBehaviour)
        {
            Debug.LogWarning("No death behaviour!");
        }

        dieBehaviour.Kill();
    }
}