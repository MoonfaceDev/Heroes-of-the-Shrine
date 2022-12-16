using System.Collections.Generic;
using UnityEngine;
using static MathUtils;

[ExecuteInEditMode]
[RequireComponent(typeof(MovableObject))]
public class Hitbox : MonoBehaviour
{
    public Vector3 size;

    private MovableObject movableObject;

    private void Awake()
    {
        movableObject = GetComponent<MovableObject>();
    }

    public Vector3 WorldPosition => movableObject.WorldPosition;

    void OnDrawGizmos()
    {
        if (movableObject && CompareTag("Barrier"))
        {
            Color lineColor = new(1.0f, 0.5f, 0.0f);
            Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
            DrawHitbox(lineColor, fillColor);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (movableObject && !CompareTag("Barrier"))
        {
            Color lineColor = new(1.0f, 0.5f, 0.0f);
            Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
            DrawHitbox(lineColor, fillColor);
        }
    }

    private void DrawHitbox(Color lineColor, Color fillColor)
    {
        DrawOutline(MovableObject.ScreenCoordinates(WorldPosition + size.z / 2 * Vector3.back),
            MovableObject.ScreenCoordinates(size + size.z * Vector3.back), lineColor);
        DrawBoth(MovableObject.ScreenCoordinates(WorldPosition + size.y / 2 * Vector3.up),
            MovableObject.ScreenCoordinates(size + size.y * Vector3.down), lineColor, fillColor);
        DrawFill(MovableObject.ScreenCoordinates(WorldPosition + WorldPosition.y * Vector3.down),
            MovableObject.ScreenCoordinates(size + size.y * Vector3.down), new Color(0, 0, 0, 0.5f));
    }

    void DrawBoth(Vector2 screenCenter, Vector2 screenSize, Color lineColor, Color fillColor)
    {
        DrawOutline(screenCenter, screenSize, lineColor);
        DrawFill(screenCenter, screenSize, fillColor);
    }

    void DrawOutline(Vector2 screenCenter, Vector2 screenSize, Color lineColor)
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireCube((Vector3)screenCenter + 0.01f * Vector3.forward,
            (Vector3)screenSize + 0.01f * Vector3.forward);
    }

    void DrawFill(Vector2 screenCenter, Vector2 screenSize, Color fillColor)
    {
        Gizmos.color = fillColor;
        Gizmos.DrawCube((Vector3)screenCenter + 0.01f * Vector3.forward, (Vector3)screenSize + 0.01f * Vector3.forward);
    }

    public bool IsInside(Vector3 point)
    {
        return (point.x >= GetLeft() && point.x <= GetRight())
               && (point.y >= GetBottom() && point.y <= GetTop())
               && (point.z >= GetFar() && point.z <= GetNear());
    }

    static bool IsBetween(float min1, float max1, float min2, float max2)
    {
        return max1 >= min2 && max2 >= min1;
    }

    public bool OverlapHitbox(Hitbox hitbox, float padding = 0)
    {
        return IsBetween(GetLeft() + padding, GetRight() - padding, hitbox.GetLeft() + padding,
                   hitbox.GetRight() - padding) &&
               IsBetween(GetBottom() + padding, GetTop() - padding, hitbox.GetBottom() + padding,
                   hitbox.GetTop() - padding) &&
               IsBetween(GetFar() + padding, GetNear() - padding, hitbox.GetFar() + padding,
                   hitbox.GetNear() - padding);
    }

    public List<Vector2> GetSegmentIntersections(Vector2 start, Vector2 end)
    {
        return LineRectangleIntersections(start, end, ToPlane(WorldPosition), ToPlane(size));
    }

    public float GetLeft()
    {
        return WorldPosition.x - size.x / 2;
    }

    public float GetRight()
    {
        return WorldPosition.x + size.x / 2;
    }

    public float GetBottom()
    {
        return WorldPosition.y - size.y / 2;
    }

    public float GetTop()
    {
        return WorldPosition.y + size.y / 2;
    }

    public float GetFar()
    {
        return WorldPosition.z - size.z / 2;
    }

    public float GetNear()
    {
        return WorldPosition.z + size.z / 2;
    }
}