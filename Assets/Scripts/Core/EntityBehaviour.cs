using System;
using System.Collections.Generic;
using System.Reflection;

public class EntityBehaviour : BaseComponent
{
    /// <summary>
    /// If <c>true</c>, the behaviour is updated (<c>Update</c> is called).
    /// Can be set to <c>false</c> multiple times, as it will change back to <c>true</c> only if it was set to <c>true</c> the same number of times.
    /// </summary>
    public bool Enabled
    {
        get => disableCount == 0;
        set
        {
            if (value)
            {
                if (disableCount > 0)
                {
                    disableCount--;
                }
            }
            else
            {
                disableCount++;
            }

            enabled = disableCount == 0;
        }
    }

    private int disableCount;

    public GameEntity Entity
    {
        get
        {
            if (!entity)
            {
                entity = GetComponentInParent<GameEntity>();
            }

            return entity;
        }
    }

    private GameEntity entity;

    protected virtual void Awake()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                                         BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<InjectBehaviourAttribute>(false) == null)
            {
                continue;
            }

            field.SetValue(this, GetBehaviour(field.GetType()));
        }
    }

    public IEnumerable<EntityBehaviour> GetBehaviours(Type type)
    {
        return Entity.GetBehaviours(type);
    }

    public EntityBehaviour GetBehaviour(Type type)
    {
        return Entity.GetBehaviour(type);
    }

    public IEnumerable<T> GetBehaviours<T>()
    {
        return Entity.GetBehaviours<T>();
    }

    public T GetBehaviour<T>()
    {
        return Entity.GetBehaviour<T>();
    }

    private void SetEnabled(bool enabledValue, IEnumerable<Type> behaviours)
    {
        foreach (var type in behaviours)
        {
            foreach (var behaviour in GetBehaviours(type))
            {
                behaviour.Enabled = enabledValue;
            }
        }
    }

    /// <summary>
    /// Enables behaviours, meaning they are updated
    /// </summary>
    /// <param name="behaviours">Behaviours to enable. All of the attached behaviours from each type will be enabled.</param>
    public void EnableBehaviours(params Type[] behaviours)
    {
        SetEnabled(true, behaviours);
    }

    /// <summary>
    /// Disables behaviours, meaning they are not updated. If a behaviour is disabled N times, it will have to be enabled N times so it enabled again.
    /// </summary>
    /// <param name="behaviours">Behaviours to disable. All of the attached behaviours from each type will be enabled.</param>
    public void DisableBehaviours(params Type[] behaviours)
    {
        SetEnabled(false, behaviours);
    }
}