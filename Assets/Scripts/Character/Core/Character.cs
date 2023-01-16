using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MovableEntity))]
public class Character : BaseComponent
{
    public PhysicalAttributes physicalAttributes;
    public Animator animator;

    [FormerlySerializedAs("movableObject")] [HideInInspector] public MovableEntity movableEntity;
    [HideInInspector] public AttackManager attackManager;

    public void Awake()
    {
        movableEntity = GetComponent<MovableEntity>();
        attackManager = GetComponent<AttackManager>();
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