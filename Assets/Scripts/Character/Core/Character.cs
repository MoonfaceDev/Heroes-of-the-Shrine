using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Shared methods and common components references for characters
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MovableEntity))]
public class Character : EntityBehaviour
{
    /// <value>
    /// Physical attributes
    /// </value>
    public CharacterStats stats;

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
        movableEntity = (MovableEntity)Entity;
        attackManager = GetBehaviour<AttackManager>();
    }

    private void SetBehavioursBlocked(bool blockedValue, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            foreach (var component in GetBehaviours(type))
            {
                var behaviour = (IPlayableBehaviour)component;
                behaviour.Blocked = blockedValue;
            }
        }
    }

    /// <summary>
    /// Blocks behaviours, meaning they cannot be played. If a behaviour is blocked N times, it will have to be unblocked N times so it can be played.
    /// </summary>
    /// <param name="types">Behaviours to block. All of the attached behaviours from each type will be blocked.</param>
    public void BlockBehaviours(params Type[] types)
    {
        SetBehavioursBlocked(true, types);
    }

    /// <summary>
    /// Unblocks behaviours, meaning they can be played
    /// </summary>
    /// <param name="types">Behaviours to unblock. All of the attached behaviours from each type will be unblocked.</param>
    public void UnblockBehaviours(params Type[] types)
    {
        SetBehavioursBlocked(false, types);
    }

    /// <summary>
    /// Stops behaviours if they where playing
    /// </summary>
    /// <param name="types">Behaviours to stop. All of the attached behaviours from each type will be stopped.</param>
    public void StopBehaviours(params Type[] types)
    {
        foreach (var type in types)
        {
            foreach (var component in GetBehaviours(type))
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
        var behaviours = GetBehaviours<T>();
        return behaviours.Any(behaviour => behaviour.Playing);
    }
}