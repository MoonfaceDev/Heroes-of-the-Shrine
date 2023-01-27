using System;
using System.Collections.Generic;
using System.Linq;

public class EntityManager : BaseComponent
{
    public static EntityManager Instance { get; private set; }

    private Dictionary<Tag, List<GameEntity>> indexes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        InitializeIndexes();
    }

    private void InitializeIndexes()
    {
        indexes = new Dictionary<Tag, List<GameEntity>>();
        foreach (Tag key in Enum.GetValues(typeof(Tag)))
        {
            indexes[key] = new List<GameEntity>();
        }
    }

    public void AddEntity(Tags tags, GameEntity entity)
    {
        foreach (var key in tags)
        {
            indexes[key].Add(entity);
        }
    }

    public void RemoveEntity(Tags tags, GameEntity entity)
    {
        foreach (var key in tags)
        {
            indexes[key].Remove(entity);
        }
    }

    public int CountEntities(Tag key)
    {
        return indexes[key].Count;
    }

    public List<GameEntity> GetEntities(Tag key)
    {
        return indexes[key];
    }

    public List<GameEntity> GetEntities(params Tag[] keys)
    {
        return keys.Aggregate(new List<GameEntity>(), (entities, key) => entities.Concat(GetEntities(key)).ToList());
    }

    public GameEntity GetEntity(Tag key)
    {
        try
        {
            return GetEntities(key)[0];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}