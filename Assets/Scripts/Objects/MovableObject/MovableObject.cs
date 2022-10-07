using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MathUtils;

public class MovableObject : MonoBehaviour
{
    public static float zScale = 0.8f;

    public GameObject figureObject;

    public Vector3 position;
    [HideInInspector]
    public Vector3 startPosition;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 acceleration;

    public event Action onStuck;

    private WalkableGrid walkableGrid;

    public void Awake()
    {
        walkableGrid = FindObjectOfType<WalkableGrid>();
    }

    private void Start()
    {
        startPosition = position;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //update position and velocity
        velocity += acceleration * Time.deltaTime;
        UpdatePosition(position + Time.deltaTime * velocity + 0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration);
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
        Hitbox[] hitboxes = FindObjectsOfType<Hitbox>();
        List<Vector3> intersections = new();
        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitbox.CompareTag("Barrier"))
            {
                List<Vector3> newIntersections = hitbox.GetSegmentIntersections(this.position, position);
                intersections.AddRange(newIntersections);
            }
        }

        if (walkableGrid)
        {
            Vector3 gridPosition = walkableGrid.GetComponent<MovableObject>().position;
            Vector3 gridSize = walkableGrid.gridWorldSize;
            intersections.AddRange(LineRectangleIntersections(ToPlane(this.position), ToPlane(position), ToPlane(gridPosition), ToPlane(gridSize)).Select(point => ToSpace(point)));
        }

        if (intersections.Count > 0)
        {
            Vector3 closest = intersections.Aggregate(position, (prev, next) =>
            {
                if (Vector3.Distance(this.position, next) < Vector3.Distance(this.position, prev))
                {
                    return next;
                }
                return prev;
            });
            this.position = closest - 0.01f * (closest - this.position).normalized;
            onStuck?.Invoke();
        } else
        {
            this.position = position;
        }
    }

    private void UpdateSortingOrder()
    {
        figureObject.GetComponent<SpriteRenderer>().sortingOrder = -1 * (Mathf.RoundToInt(position.z * 100f) * 10);
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