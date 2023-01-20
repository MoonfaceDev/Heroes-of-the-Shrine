using System;
using UnityEngine;

/// <summary>
/// Base class for all character behaviours. Contains useful methods and references to other components.
/// </summary>
[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : BaseComponent
{
    public Character Character { get; private set; }

    public MovableEntity MovableEntity => Character.movableEntity;
    protected Animator Animator => Character.animator;
    public AttackManager AttackManager => Character.attackManager;

    /// <summary>
    /// If <c>true</c>, the behaviour can be played
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

    public virtual void Awake()
    {
        Character = GetComponent<Character>();
    }

    protected void EnableBehaviours(params Type[] behaviours)
    {
        Character.EnableBehaviours(behaviours);
    }

    protected void DisableBehaviours(params Type[] behaviours)
    {
        Character.DisableBehaviours(behaviours);
    }

    protected void StopBehaviours(params Type[] behaviours)
    {
        Character.StopBehaviours(behaviours);
    }

    protected bool IsPlaying<T>() where T : IPlayableBehaviour
    {
        return Character.IsPlaying<T>();
    }
}