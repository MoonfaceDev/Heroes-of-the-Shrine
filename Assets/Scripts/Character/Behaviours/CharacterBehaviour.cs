using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : MonoBehaviour
{
    public static float gravityAcceleration = 20;

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
                disableCount--;
            }
            else
            {
                disableCount++;
            }
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

    protected void EnableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(true, behaviours);
    }

    protected void DisableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(false, behaviours);
    }

    protected void StopBehaviours(params Type[] behaviours)
    {
        foreach (Type type in behaviours)
        {
            foreach (PlayableBehaviour behaviour in GetComponents(type))
            {
                behaviour.Stop();
            }
        }
    }

    protected bool IsPlaying(Type type)
    {
        PlayableBehaviour behaviour = GetComponent(type) as PlayableBehaviour;
        return behaviour && behaviour.Playing;
    }

    protected bool AllStopped(params Type[] types)
    {
        return types.Select(type => GetComponents(type)).All(behaviours => behaviours.All(behaviour => !(behaviour as PlayableBehaviour).Playing));
    }
}