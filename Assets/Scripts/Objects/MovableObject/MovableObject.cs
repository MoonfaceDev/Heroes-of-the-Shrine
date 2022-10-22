using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MathUtils;

public class MovableObject : MonoBehaviour
{
    public static float zScale = 0.8f;

    public Renderer figureObject;

    public Vector3 position;
    [HideInInspector]
    public Vector3 startPosition;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 acceleration;

    public Vector3 WorldPosition
    {
        get
        {
            MovableObject[] movableObjects = GetComponentsInParent<MovableObject>();
            return movableObjects.Aggregate(Vector3.zero, (total, next) =>
            {
                return total + next.position;
            });
        }
    }

    public event Action OnStuck;

    private WalkableGrid walkableGrid;

    public void Awake()
    {
        walkableGrid = FindObjectOfType<WalkableGrid>();
        startPosition = position;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        OnStuck += () =>
        {
            velocity = Vector3.zero + velocity.y * Vector3.up;
            acceleration = Vector3.zero + acceleration.y * Vector3.up;
        };
    }

    // Update is called once per frame
    void Update()
    {
        //update position and velocity
        velocity += acceleration * Time.deltaTime;
        if (!CompareTag("Barrier")) {
            Vector3 wantedPosition = position + Time.deltaTime * velocity + 0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration;
            UpdatePosition(position + Vector3.right * (wantedPosition.x - position.x));
            UpdatePosition(position + Vector3.up * (wantedPosition.y - position.y));
            UpdatePosition(position + Vector3.forward * (wantedPosition.z - position.z));
        }
        //update position in scene
        transform.position = GroundScreenCoordinates(position);
        if (figureObject)
        {
            figureObject.transform.localPosition = ScreenCoordinates(position.y * Vector3.up);
            UpdateSortingOrder();
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        Vector2 previousGroundPosition = ToPlane(position);
        Vector2 groundPosition = ToPlane(position);
        Hitbox[] barriers = HitboxManager.Instance.Barriers;
        List<Vector2> intersections = new();
        foreach (Hitbox barrier in barriers)
        {
            List<Vector2> newIntersections = barrier.GetSegmentIntersections(previousGroundPosition, groundPosition);
            intersections.AddRange(newIntersections);
        }

        if (walkableGrid)
        {
            Vector3 gridPosition = walkableGrid.GetComponent<MovableObject>().position;
            Vector3 gridSize = walkableGrid.gridWorldSize;
            intersections.AddRange(LineRectangleIntersections(previousGroundPosition, groundPosition, ToPlane(gridPosition), ToPlane(gridSize)));
        }

        if (intersections.Count > 0)
        {
            Vector2 closest = intersections.Aggregate(groundPosition, (prev, next) =>
            {
                if (Vector2.Distance(previousGroundPosition, next) < Vector2.Distance(previousGroundPosition, prev))
                {
                    return next;
                }
                return prev;
            });
            if (Vector2.Distance(previousGroundPosition, closest) > 0.001f)
            {
                position = ToSpace(closest - 0.001f * (groundPosition - previousGroundPosition).normalized) + position.y * Vector3.up;
            }
            else
            {
                position = this.position;
            }
            OnStuck?.Invoke();
        }
        this.position.y = position.y;
        if (IsValidPosition(position))
        {
            this.position = position;
        }
    }

    public bool IsValidPosition(Vector3 position)
    {
        if (HitboxManager.Instance.GetOverlappingBarriers(position).Length > 0)
        {
            return false;
        }
        if (walkableGrid && !walkableGrid.IsInside(position))
        {
            return false;
        }
        return true;
    }

    private void UpdateSortingOrder()
    {
        figureObject.sortingOrder = -1 * Mathf.RoundToInt(WorldPosition.z * 100f) * 10;
    }

    public float Distance(Vector3 point)
    {
        return (position - point).magnitude;
    }

    public float GroundDistance(Vector3 point)
    {
        Vector3 distance = position - point;
        distance.y = 0;
        return distance.magnitude;
    }

    public static Vector3 WorldCoordinates(Vector3 v)
    {
        return new Vector3(v.x, 0, v.y / zScale);
    }

    // Without elevation
    public static Vector3 GroundScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.z * zScale, 0);
    }
    
    // With elevation
    public static Vector3 ScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.y + v.z * zScale, 0);
    }
}