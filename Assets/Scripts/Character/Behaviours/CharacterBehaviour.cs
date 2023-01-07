using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : BaseComponent
{
    public Character Character { get; private set; }

    public MovableObject MovableObject => Character.movableObject;
    protected Animator Animator => Character.animator;
    protected AttackManager AttackManager => Character.attackManager;

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

    private void SetBehavioursEnabled(bool enabledValue, IEnumerable<Type> behaviours)
    {
        foreach (var type in behaviours)
        {
            foreach (var component in GetComponents(type))
            {
                var behaviour = (CharacterBehaviour)component;
                behaviour.Enabled = enabledValue;
            }
        }
    }

    public void EnableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(true, behaviours);
    }

    public void DisableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(false, behaviours);
    }

    public void StopBehaviours(params Type[] behaviours)
    {
        foreach (var type in behaviours)
        {
            foreach (var component in GetComponents(type))
            {
                var behaviour = (IPlayableBehaviour)component;
                behaviour.Stop();
            }
        }
    }

    public bool IsPlaying<T>() where T : IPlayableBehaviour
    {
        var behaviours = GetComponents<T>();
        return behaviours.Any(behaviour => behaviour.Playing);
    }
}