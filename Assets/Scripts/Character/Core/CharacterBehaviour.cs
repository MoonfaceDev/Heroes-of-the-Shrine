using System;
using UnityEngine;

/// <summary>
/// Base class for all character behaviours. Contains useful methods and references to other components.
/// </summary>
public abstract class CharacterBehaviour : EntityBehaviour
{
    /// <value>
    /// Character reference
    /// </value>
    [field: InjectBehaviour]
    protected Character Character { get; }

    /// <value>
    /// Animator of the figure (related <see cref="SpriteRenderer"/>)
    /// </value>
    protected Animator Animator => Character.animator;

    /// <value>
    /// Attached <see cref="MovableEntity"/>
    /// </value>
    public MovableEntity MovableEntity => Character.MovableEntity;

    /// <value>
    /// Attached <see cref="AttackManager"/>, possibly null
    /// </value>
    public AttackManager AttackManager => Character.attackManager;

    /// <summary>
    /// Blocks behaviours, meaning they cannot be played. If a behaviour is blocked N times, it will have to be unblocked N times so it can be played.
    /// </summary>
    /// <param name="behaviours">Behaviours to block. All of the attached behaviours from each type will be blocked.</param>
    protected void BlockBehaviours(params Type[] behaviours)
    {
        Character.BlockBehaviours(behaviours);
    }

    /// <summary>
    /// Unblocks behaviours, meaning they can be played
    /// </summary>
    /// <param name="behaviours">Behaviours to unblock. All of the attached behaviours from each type will be unblocked.</param>
    protected void UnblockBehaviours(params Type[] behaviours)
    {
        Character.UnblockBehaviours(behaviours);
    }

    /// <summary>
    /// Stops behaviours if they where playing
    /// </summary>
    /// <param name="types">Types of behaviours to stop. All of the attached behaviours from each type will be stopped.</param>
    protected void StopBehaviours(params Type[] types)
    {
        Character.StopBehaviours(types);
    }

    /// <summary>
    /// Checks if any of the attached behaviours from type T are playing
    /// </summary>
    /// <typeparam name="T">Type of the behaviour</typeparam>
    /// <returns><c>true</c> if any is playing</returns>
    protected bool IsPlaying<T>() where T : IPlayableBehaviour
    {
        return Character.IsPlaying<T>();
    }
}