﻿using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// <see cref="GameEntity"/> with moving capabilities
/// </summary>
[ExecuteInEditMode]
public class MovableEntity : GameEntity
{
    /// <value>
    /// Velocity in relative coordinates
    /// </value>
    [ShowDebug] public Vector3 velocity = Vector3.zero;

    /// <value>
    /// Acceleration in relative coordinates
    /// </value>
    [ShowDebug] public Vector3 acceleration = Vector3.zero;

    /// <value>
    /// Invoked when entity tried to move into a barrier
    /// </value>
    public event Action OnStuck;

    /// <value>
    /// Invoked when entity landed on the ground (y=0) 
    /// </value>
    public event Action OnLand;

    private WalkableGrid walkableGrid;
    private CameraMovement cameraMovement;

    private const float MaxStepSize = 0.01f;

    protected override void Awake()
    {
        base.Awake();

        walkableGrid = FindObjectOfType<WalkableGrid>();
        if (Camera.main != null) cameraMovement = Camera.main.GetComponent<CameraMovement>();

        OnLand += () =>
        {
            position.y = 0;
            velocity.y = 0;
            acceleration.y = 0;
        };
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Update velocity
        velocity += acceleration * Time.deltaTime;

        // Update position
        var wantedPosition = position + Time.deltaTime * velocity +
                             0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration;
        UpdatePosition(wantedPosition);

        // Update scene
        UpdateTransform();
    }

    public void UpdateWorldPosition(Vector3 newPosition)
    {
        UpdatePosition(parent ? parent.TransformToRelative(newPosition) : newPosition);
    }

    /// <summary>
    /// Move the entity to a new position. If there's a barrier in the way, the entity will stop before hitting hit
    /// </summary>
    /// <param name="newPosition">Target position in relative coordinates</param>
    public void UpdatePosition(Vector3 newPosition)
    {
        if (!Application.isPlaying)
        {
            position = newPosition;
            return;
        }

        var offset = newPosition - position;
        var direction = offset.normalized;
        var distance = offset.magnitude;
        var steps = distance / MaxStepSize + 1;
        var step = distance / steps * direction;

        var stuck = false;

        for (var i = 0; i < steps; i++)
        {
            var xValid = AddOffset(step.x * Vector3.right);
            var yValid = AddOffset(step.y * Vector3.up);
            var zValid = AddOffset(step.z * Vector3.forward);
            
            if (!xValid || !yValid || !zValid)
            {
                stuck = true;
            }
        }

        if (stuck)
        {
            OnStuck?.Invoke();
        }

        if (IsLanded(newPosition))
        {
            OnLand?.Invoke();
        }
    }

    private bool AddOffset(Vector3 offset)
    {
        var newPosition = position + offset;
        if (!IsValidPosition(newPosition))
        {
            return false;
        }
        position = newPosition;
        return true;
    }

    private bool IsValidPosition(Vector3 newPosition)
    {
        if (IsLanded(newPosition))
        {
            return false;
        }
        
        if (EntityManager.Instance.GetEntities(Tag.Barrier)
            .Any(barrier => barrier.GetBehaviour<Hitbox>().IsInside(newPosition)))
        {
            return false;
        }

        if (walkableGrid && !walkableGrid.IsInside(newPosition))
        {
            return false;
        }

        if (walkableGrid && cameraMovement && tags.Contains(Tag.Player) &&
            (newPosition.x < cameraMovement.border.xMin || newPosition.x > cameraMovement.border.xMax))
        {
            return false;
        }

        return true;
    }

    private static bool IsLanded(Vector3 newPosition)
    {
        return newPosition.y < 0;
    }
}