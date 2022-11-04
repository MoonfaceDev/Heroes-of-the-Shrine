using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : MonoBehaviour
{
    public Character Character => character;
    public Animator Animator => character.animator;
    public EventManager EventManager => character.eventManager;
    public MovableObject MovableObject => character.movableObject;
    public int LookDirection
    {
        get => character.LookDirection;
        set
        {
            character.LookDirection = value;
        }
    }

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

    private Character character;
    private int disableCount;

    public virtual void Awake()
    {
        character = GetComponent<Character>();
    }

    private void SetBehavioursEnabled(bool enabled, Type[] behaviours)
    {
        foreach (Type type in behaviours)
        {
            foreach (CharacterBehaviour behaviour in GetComponents(type))
            {
                behaviour.Enabled = enabled;
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
        foreach (Type type in behaviours)
        {
            foreach (PlayableBehaviour behaviour in GetComponents(type))
            {
                behaviour.Stop();
            }
        }
    }

    public bool IsPlaying(Type type)
    {
        PlayableBehaviour behaviour = GetComponent(type) as PlayableBehaviour;
        return behaviour && behaviour.Playing;
    }

    public bool AllStopped(params Type[] types)
    {
        return types.Select(type => GetComponents(type)).All(behaviours => behaviours.All(behaviour => !(behaviour as PlayableBehaviour).Playing));
    }
}