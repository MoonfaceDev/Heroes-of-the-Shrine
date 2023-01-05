using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : MonoBehaviour
{
    public Character Character { get; private set; }

    protected Animator Animator => Character.animator;
    protected EventManager EventManager => Character.eventManager;
    public MovableObject MovableObject => Character.movableObject;

    public bool Enabled
    {
        get => disableCount == 0;
        set {
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

    private void SetBehavioursEnabled(bool enabledValue, Type[] behaviours)
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
                var behaviour = (PlayableBehaviour)component;
                behaviour.Stop();
            }
        }
    }

    public bool IsPlaying(Type type)
    {
        var behaviour = GetComponent(type) as PlayableBehaviour;
        return behaviour && behaviour.Playing;
    }

    public bool AllStopped(params Type[] types)
    {
        return types.Select(GetComponents).All(behaviours => behaviours.All(behaviour => !((PlayableBehaviour)behaviour).Playing));
    }
}