using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Shared methods and common components references for characters
/// </summary>
[RequireComponent(typeof(MovableEntity))]
public class Character : BaseComponent
{
    /// <value>
    /// Physical attributes
    /// </value>
    public PhysicalAttributes physicalAttributes;
    
    /// <value>
    /// Animator of the figure (related <see cref="SpriteRenderer"/>)
    /// </value>
    public Animator animator;
    
    /// <value>
    /// Attached <see cref="MovableEntity"/>
    /// </value>
    [HideInInspector] public MovableEntity movableEntity;
    
    /// <value>
    /// Attached <see cref="AttackManager"/>, possibly null
    /// </value>
    [HideInInspector] public AttackManager attackManager;

    private void Awake()
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

    /// <summary>
    /// Enables behaviours, meaning they can be played
    /// </summary>
    /// <param name="behaviours">Behaviours to enable. All of the attached behaviours from each type will be enabled.</param>
    public void EnableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(true, behaviours);
    }

    /// <summary>
    /// Disables behaviours, meaning they cannot be played. If a behaviour is disabled N times, it will have to be enabled N times so it can be played.
    /// </summary>
    /// <param name="behaviours">Behaviours to disable. All of the attached behaviours from each type will be enabled.</param>
    public void DisableBehaviours(params Type[] behaviours)
    {
        SetBehavioursEnabled(false, behaviours);
    }

    /// <summary>
    /// Stops behaviours if they where playing
    /// </summary>
    /// <param name="behaviours">Behaviours to stop. All of the attached behaviours from each type will be stopped.</param>
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

    /// <summary>
    /// Checks if any of the attached behaviours from type T are playing
    /// </summary>
    /// <typeparam name="T">Type of the behaviour</typeparam>
    /// <returns><c>true</c> if any is playing</returns>
    public bool IsPlaying<T>() where T : IPlayableBehaviour
    {
        var behaviours = GetComponents<T>();
        return behaviours.Any(behaviour => behaviour.Playing);
    }
}