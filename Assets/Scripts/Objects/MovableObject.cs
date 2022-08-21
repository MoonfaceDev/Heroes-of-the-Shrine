using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public static float zScale = 0.8f;

    public GameObject figureObject;

    public Vector3 startPosition;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 acceleration;

    private void Start()
    {
        position = startPosition;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 previousPosition = position;
        Vector3 previousVelocity = velocity;
        //update position and velocity
        velocity += acceleration * Time.deltaTime;
        position += Time.deltaTime * velocity + 0.5f * Mathf.Pow(Time.deltaTime, 2) * acceleration;
        //handle collision with barriers
        AvoidCollisions(previousPosition, previousVelocity);
        //update position in scene
        transform.position = GroundScreenCoordinates(position);
        figureObject.transform.localPosition = ScreenCoordinates(position.y * Vector3.up);
        UpdateSortingOrder();
    }

    private void AvoidCollisions(Vector3 previousPosition, Vector3 previousVelocity)
    {
        BarrierRectangle[] rectangles = FindObjectsOfType<BarrierRectangle>();
        foreach (BarrierRectangle rectangle in rectangles)
        {
            if (rectangle.IsInside(position))
            {
                if (!rectangle.IsInside(new Vector3(previousPosition.x, position.y, position.z)))
                {
                    position.x = previousPosition.x;
                    velocity.x = 0;
                }
                else if (!rectangle.IsInside(new Vector3(position.x, position.y, previousPosition.z)))
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
                return;
            }
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