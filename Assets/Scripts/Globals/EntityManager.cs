using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Global store of entities, indexed with tags. Use this instead of expensive methods such as <see cref="UnityEngine.GameObject.Find"/>.
/// </summary>
public class EntityManager : BaseComponent
{
    /// <value>
    /// Instance of the singleton
    /// </value>
    public static EntityManager Instance { get; private set; }

    private Dictionary<Tag, HashSet<GameEntity>> indexes;

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
        indexes = new Dictionary<Tag, HashSet<GameEntity>>();
        foreach (Tag key in Enum.GetValues(typeof(Tag)))
        {
            indexes[key] = new HashSet<GameEntity>();
        }
    }

    /// <summary>
    /// Adds a <see cref="GameEntity"/> to the store
    /// </summary>
    /// <param name="entity">Entity to add</param>
    public void AddEntity(GameEntity entity)
    {
        foreach (var key in entity.tags)
        {
            indexes[key].Add(entity);
        }
    }

    /// <summary>
    /// Removes a <see cref="GameEntity"/> from the store
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    public void RemoveEntity(GameEntity entity)
    {
        foreach (var key in entity.tags)
        {
            indexes[key].Remove(entity);
        }
    }

    /// <summary>
    /// Count how many entities have a certain tag
    /// </summary>
    /// <param name="key">Tag to check</param>
    /// <returns>Count of entities have that tag</returns>
    public int CountEntities(Tag key)
    {
        return indexes[key].Count;
    }

    /// <summary>
    /// Get all entities that have a certain tag
    /// </summary>
    /// <param name="key">Tag to check</param>
    /// <returns>All entities having that tag</returns>
    public HashSet<GameEntity> GetEntities(Tag key)
    {
        return indexes[key];
    }

    /// <summary>
    /// Get all entities that have one of a list of tags
    /// </summary>
    /// <param name="keys">List of tags to check</param>
    /// <returns>All entities having any of these tags</returns>
    public IEnumerable<GameEntity> GetEntities(params Tag[] keys)
    {
        return keys.Aggregate<Tag, IEnumerable<GameEntity>>(new HashSet<GameEntity>(),
            (entities, key) => entities.Union(GetEntities(key)));
    }

    /// <summary>
    /// Get one entity that has a certain tag
    /// </summary>
    /// <param name="key">Tag to check</param>
    /// <returns>Single entity having this tag. If there is none, then <c>null</c> is returned</returns>
    public GameEntity GetEntity(Tag key)
    {
        return GetEntities(key).SingleOrDefault();
    }
}