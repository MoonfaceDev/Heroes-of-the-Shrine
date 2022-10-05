using System;
using UnityEngine;

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
        Vector3 previousPosition = position;
        //update position and velocity
        velocity += acceleration * Time.deltaTime;
        position += Time.deltaTime * velocity + 0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration;
        //handle collision with barriers
        bool isStuck = AvoidCollisions(previousPosition);
        if (isStuck)
        {
            onStuck?.Invoke();
        }
        //update position in scene
        transform.position = GroundScreenCoordinates(position);
        if (figureObject)
        {
            figureObject.transform.localPosition = ScreenCoordinates(position.y * Vector3.up);
            UpdateSortingOrder();
        }
    }

    private bool AvoidCollisions(Vector3 previousPosition)
    {
        Hitbox[] hitboxes = FindObjectsOfType<Hitbox>();
        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitbox.CompareTag("Barrier") && hitbox.IsInside(position))
            {
                if (!hitbox.IsInside(new Vector3(previousPosition.x, position.y, position.z)))
                {
                    position.x = previousPosition.x;
                    velocity.x = 0;
                }
                else if (!hitbox.IsInside(new Vector3(position.x, position.y, previousPosition.z)))
                {
                    position.z = previousPosition.z;
                    velocity.z = 0;
                }
                else
                {
                    position.x = previousPosition.x;
                    velocity.x = 0;
                    position.z = previousPosition.z;
                    velocity.z = 0;
                }
                return true;
            }
        }

        if (walkableGrid)
        {
            Vector3 gridPosition = walkableGrid.GetComponent<MovableObject>().position;
            Vector3 gridSize = walkableGrid.gridWorldSize;
            bool xOutside = position.x < gridPosition.x || position.x > gridPosition.x + gridSize.x;
            bool zOutside = position.z < gridPosition.z || position.z > gridPosition.z + gridSize.z;
            if (xOutside && zOutside)
            {
                position.x = previousPosition.x;
                velocity.x = 0;
                position.z = previousPosition.z;
                velocity.z = 0;
            }
            else if (xOutside)
            {
                position.x = previousPosition.x;
                velocity.x = 0;
            }
            else if (zOutside)
            {
                position.z = previousPosition.z;
                velocity.z = 0;
            }
            if (xOutside || zOutside)
            {
                return true;
            }
        }

        return false;
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