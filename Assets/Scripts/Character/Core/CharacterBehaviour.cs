using System;
using UnityEngine;

/// <summary>
/// Base class for all character behaviours. Contains useful methods and references to other components.
/// </summary>
[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : BaseComponent
{
    /// <value>
    /// Character reference
    /// </value>
    public Character Character { get; private set; }

    /// <value>
    /// Animator of the figure (related <see cref="SpriteRenderer"/>)
    /// </value>
    protected Animator Animator => Character.animator;

    /// <value>
    /// Attached <see cref="MovableEntity"/>
    /// </value>
    public MovableEntity MovableEntity => Character.movableEntity;

    /// <value>
    /// Attached <see cref="AttackManager"/>, possibly null
    /// </value>
    public AttackManager AttackManager => Character.attackManager;

    /// <summary>
    /// If <c>true</c>, the behaviour can be played.
    /// Can be set to <c>false</c> multiple times, as it will change back to <c>true</c> only if it was set to <c>true</c> the same number of times.
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

    protected virtual void Awake()
    {
        Character = GetComponent<Character>();
    }

    /// <summary>
    /// Enables behaviours, meaning they can be played
    /// </summary>
    /// <param name="behaviours">Behaviours to enable. All of the attached behaviours from each type will be enabled.</param>
    protected void EnableBehaviours(params Type[] behaviours)
    {
        Character.EnableBehaviours(behaviours);
    }

    /// <summary>
    /// Disables behaviours, meaning they cannot be played. If a behaviour is disabled N times, it will have to be enabled N times so it can be played.
    /// </summary>
    /// <param name="behaviours">Behaviours to disable. All of the attached behaviours from each type will be enabled.</param>
    protected void DisableBehaviours(params Type[] behaviours)
    {
        Character.DisableBehaviours(behaviours);
    }

    /// <summary>
    /// Stops behaviours if they where playing
    /// </summary>
    /// <param name="behaviours">Behaviours to stop. All of the attached behaviours from each type will be stopped.</param>
    protected void StopBehaviours(params Type[] behaviours)
    {
        Character.StopBehaviours(behaviours);
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