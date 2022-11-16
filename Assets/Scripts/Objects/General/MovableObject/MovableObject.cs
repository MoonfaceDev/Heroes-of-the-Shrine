using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MathUtils;

public class MovableObject : MonoBehaviour
{
    private const float ZScale = 0.8f;

    public MovableObject parent;
    public Renderer figureObject;

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale = Vector3.one;

    [ShowDebug] public Vector3 velocity;
    [ShowDebug] public Vector3 acceleration;

    public Vector3 WorldPosition {
        get => TransformToWorld(position);
        set => position = TransformToRelative(value);
    }

    public Vector3 GroundWorldPosition => WorldPosition - WorldPosition.y * Vector3.up;

    public event Action OnStuck;
    public event Action OnLand;

    private WalkableGrid walkableGrid;
    private CameraMovement cameraMovement;

    public void Awake()
    {
        walkableGrid = FindObjectOfType<WalkableGrid>();
        if (Camera.main != null) cameraMovement = Camera.main.GetComponent<CameraMovement>();
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
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
    private void Update()
    {
        //update position and velocity
        velocity += acceleration * Time.deltaTime;
        if (!CompareTag("Barrier")) {
            var wantedPosition = position + Time.deltaTime * velocity + 0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration;
            UpdatePosition(position + Vector3.right * (wantedPosition.x - position.x));
            UpdatePosition(position + Vector3.up * (wantedPosition.y - position.y));
            UpdatePosition(position + Vector3.forward * (wantedPosition.z - position.z));
        }
        //update position in scene
        transform.localPosition = GroundScreenCoordinates(position);
        transform.localRotation = rotation;
        transform.localScale = scale;
        if (figureObject)
        {
            figureObject.transform.localPosition = ScreenCoordinates(WorldPosition.y * Vector3.up);
            UpdateSortingOrder();
        }
    }

    private Vector3 TransformToWorld(Vector3 relativePoint)
    {
        if (!parent)
        {
            return relativePoint;
        }
        return parent.WorldPosition + Vector3.Scale(parent.rotation * relativePoint, parent.scale);
    }

    private Vector3 TransformToRelative(Vector3 worldPoint)
    {
        if (!parent)
        {
            return worldPoint;
        }
        return Vector3.Scale(worldPoint - parent.WorldPosition, Quaternion.Inverse(parent.rotation) * new Vector3(1 / parent.scale.x, 1 / parent.scale.y, 1 / parent.scale.z));
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        var previousGroundPosition = ToPlane(WorldPosition);
        var groundPosition = ToPlane(TransformToWorld(newPosition));
        var intersections = GetIntersections(previousGroundPosition, groundPosition);

        if (intersections.Count > 0)
        {
            var closest = intersections.Aggregate(groundPosition, (prev, next) => Vector2.Distance(previousGroundPosition, next) < Vector2.Distance(previousGroundPosition, prev) ? next : prev);
            newPosition = ToSpace(TransformToRelative(closest - 0.001f * (groundPosition - previousGroundPosition).normalized), newPosition.y);
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
        var gridPosition = ToPlane(walkableGrid.movableObject.WorldPosition);
        var gridSize = ToPlane(walkableGrid.gridWorldSize);
        return LineRectangleIntersections(previousGroundPosition, groundPosition, gridPosition, gridSize);
    }

    private IEnumerable<Vector2> GetCameraBorderIntersections(Vector2 previousGroundPosition, Vector2 groundPosition)
    {
        var border = cameraMovement.border;
        var gridPosition = ToPlane(walkableGrid.movableObject.WorldPosition);
        var gridSize = ToPlane(walkableGrid.gridWorldSize);
        return LineRectangleIntersections(previousGroundPosition, groundPosition, new Vector2(border.xMin, gridPosition.y), new Vector2(border.width, gridSize.y));
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
        if (walkableGrid && cameraMovement && CompareTag("Player") && (newPosition.x < cameraMovement.border.xMin || newPosition.x > cameraMovement.border.xMax))
        {
            return false;
        }
        return true;
    }

    private void UpdateSortingOrder()
    {
        figureObject.sortingOrder = -1 * Mathf.RoundToInt(WorldPosition.z * 100f) * 10;
    }

    public float GroundDistance(Vector3 point)
    {
        var distance = WorldPosition - point;
        distance.y = 0;
        return distance.magnitude;
    }

    public static Vector3 WorldCoordinates(Vector3 v)
    {
        return new Vector3(v.x, 0, v.y / ZScale);
    }

    // Without elevation
    public static Vector3 GroundScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.z * ZScale, 0);
    }
    
    // With elevation
    public static Vector3 ScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.y + v.z * ZScale, 0);
    }
}