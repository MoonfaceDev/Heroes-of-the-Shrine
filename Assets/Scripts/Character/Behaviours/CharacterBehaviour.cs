using System;
using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : MonoBehaviour
{
    public static float gravityAcceleration = 20;

    public Animator animator => character.animator;
    public EventManager eventManager => character.eventManager;
    public MovableObject movableObject => character.movableObject;
    public int lookDirection
    {
        get => character.lookDirection;
        set
        {
            character.lookDirection = value;
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
}