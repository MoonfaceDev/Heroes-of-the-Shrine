using System;
using System.Collections.Generic;
using UnityEngine;

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

    public GameEntity[] GetEntities(Tag key)
    {
        return indexes[key].ToArray();
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