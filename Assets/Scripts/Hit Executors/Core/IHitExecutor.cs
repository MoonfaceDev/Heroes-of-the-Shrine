using UnityEngine;

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {
    }
}


public record Hit(IHittable Victim, Vector3 Point, BaseAttack Source = null, Rotation Direction = null)
{
    public Rotation Direction { get; init; } = Direction ?? Rotation.Normal;

    public Hit(Collision collision, BaseAttack source = null, Rotation direction = null) : this(collision.Other,
        collision.Point, source, direction)
    {
    }
}

/// <summary>
/// Interface for hit executors
/// </summary>
public interface IHitExecutor
{
    /// <summary>
    /// Performs hit logic on an object
    /// </summary>
    /// <param name="hit">Hit context</param>
    public void Execute(Hit hit);
}