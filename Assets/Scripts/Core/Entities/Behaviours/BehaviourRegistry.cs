
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BehaviourRegistry
{
    private readonly Dictionary<Type, HashSet<EntityBehaviour>> indexes = new();
    
    public void RegisterBehaviours(Transform currentObject)
    {
        foreach (var behaviour in currentObject.GetComponents<EntityBehaviour>())
        {
            RegisterBehaviour(behaviour);
        }

        foreach (Transform child in currentObject)
        {
            if (child.GetComponent<GameEntity>() == null)
            {
                RegisterBehaviours(child);
            }
        }
    }
    
    private void RegisterBehaviour(EntityBehaviour behaviour)
    {
        var currentType = behaviour.GetType();
        while (currentType != null)
        {
            AddToIndex(currentType, behaviour);
            currentType = currentType.BaseType;
        }

        foreach (var type in behaviour.GetType().GetInterfaces())
        {
            AddToIndex(type, behaviour);
        }
    }
    
    private void AddToIndex(Type type, EntityBehaviour behaviour)
    {
        if (!indexes.ContainsKey(type))
        {
            indexes[type] = new HashSet<EntityBehaviour>();
        }

        indexes[type].Add(behaviour);
    }
    
    public IEnumerable<EntityBehaviour> GetBehaviours(Type type, bool exactType = false)
    {
        var behaviourSet = indexes.GetValueOrDefault(type, new HashSet<EntityBehaviour>());
        return exactType ? behaviourSet.Where(behaviour => behaviour.GetType() == type) : behaviourSet;
    }

    public EntityBehaviour GetBehaviour(Type type, bool exactType = false)
    {
        return GetBehaviours(type, exactType).SingleOrDefault();
    }

    public IEnumerable<T> GetBehaviours<T>(bool exactType = false)
    {
        return GetBehaviours(typeof(T), exactType).Select(behaviour => (T)(object)behaviour);
    }

    public T GetBehaviour<T>(bool exactType = false)
    {
        return GetBehaviours<T>(exactType).SingleOrDefault();
    }
}