using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MathUtils;

[ExecuteInEditMode]
public class MovableObject : GameEntity
{
    [ShowDebug] public Vector3 velocity = Vector3.zero;
    [ShowDebug] public Vector3 acceleration = Vector3.zero;

    public event Action OnStuck;
    public event Action OnLand;

    private WalkableGrid walkableGrid;
    private CameraMovement cameraMovement;

    protected override void Awake()
    {
        base.Awake();

        walkableGrid = FindObjectOfType<WalkableGrid>();
        if (Camera.main != null) cameraMovement = Camera.main.GetComponent<CameraMovement>();

        OnStuck += () =>
        {
            velocity = Vector3.zero + velocity.y * Vector3.up;
            acceleration = Vector3.zero + acceleration.y * Vector3.up;
        };

        OnLand += () =>
        {
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
        UpdatePosition(position + Vector3.right * (wantedPosition.x - position.x));
        UpdatePosition(position + Vector3.up * (wantedPosition.y - position.y));
        UpdatePosition(position + Vector3.forward * (wantedPosition.z - position.z));

        // Update scene
        UpdateTransform();
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        if (!Application.isPlaying)
        {
            position = newPosition;
            return;
        }

        var previousGroundPosition = ToPlane(WorldPosition);
        var groundPosition = ToPlane(parent ? parent.TransformToWorld(newPosition) : newPosition);
        var intersections = GetIntersections(previousGroundPosition, groundPosition);

        if (intersections.Count > 0)
        {
            var closest = intersections.Aggregate(groundPosition, (prev, next) =>
                Vector2.Distance(previousGroundPosition, next) < Vector2.Distance(previousGroundPosition, prev)
                    ? next
                    : prev
            );
            var newWorldGroundPosition = closest - 0.001f * (groundPosition - previousGroundPosition).normalized;
            newPosition = ToSpace(parent ? parent.TransformToRelative(newWorldGroundPosition) : newWorldGroundPosition,
                newPosition.y);
            OnStuck?.Invoke();
        }

        if (newPosition.y < 0)
        {
            newPosition.y = 0;
            OnLand?.Invoke();
        }

        position.y = newPosition.y;
        if (IsValidPosition(newPosition))
        {
            position = newPosition;
        }
    }

    private List<Vector2> GetIntersections(Vector2 previousGroundPosition, Vector2 groundPosition)
    {
        var intersections = GetBarrierIntersections(previousGroundPosition, groundPosition);
        if (!walkableGrid) return intersections;
        intersections.AddRange(GetWalkableGridIntersections(previousGroundPosition, groundPosition));
        if (cameraMovement && CompareTag("Player"))
        {
            intersections.AddRange(GetCameraBorderIntersections(previousGroundPosition, groundPosition));
        }

        return intersections;
    }

    private static List<Vector2> GetBarrierIntersections(Vector2 previousGroundPosition, Vector2 groundPosition)
    {
        var barriers = CachedObjectsManager.Instance.GetObjects<Hitbox>("Barrier").ToArray();
        List<Vector2> intersections = new();
        foreach (var barrier in barriers)
        {
            var newIntersections = barrier.GetSegmentIntersections(previousGroundPosition, groundPosition);
            intersections.AddRange(newIntersections);
        }

        return intersections;
    }

    private IEnumerable<Vector2> GetWalkableGridIntersections(Vector2 previousGroundPosition, Vector2 groundPosition)
    {
        var gridPosition = ToPlane(walkableGrid.entity.WorldPosition);
        var gridSize = ToPlane(walkableGrid.gridWorldSize);
        return LineRectangleIntersections(previousGroundPosition, groundPosition, gridPosition + gridSize / 2,
            gridSize);
    }

    private IEnumerable<Vector2> GetCameraBorderIntersections(Vector2 previousGroundPosition, Vector2 groundPosition)
    {
        var border = cameraMovement.border;
        var gridPosition = ToPlane(walkableGrid.entity.WorldPosition);
        var gridSize = ToPlane(walkableGrid.gridWorldSize);
        return LineRectangleIntersections(previousGroundPosition, groundPosition,
            new Vector2(border.center.x, gridPosition.y + gridSize.y / 2), new Vector2(border.width, gridSize.y));
    }

    private bool IsValidPosition(Vector3 newPosition)
    {
        if (CachedObjectsManager.Instance.GetObjects<Hitbox>("Barrier").Any(hitbox => hitbox.IsInside(newPosition)))
        {
            return false;
        }

        if (walkableGrid && !walkableGrid.IsInside(newPosition))
        {
            return false;
        }

        if (walkableGrid && cameraMovement && CompareTag("Player") && (newPosition.x < cameraMovement.border.xMin ||
                                                                       newPosition.x > cameraMovement.border.xMax))
        {
            return false;
        }

        return true;
    }
}