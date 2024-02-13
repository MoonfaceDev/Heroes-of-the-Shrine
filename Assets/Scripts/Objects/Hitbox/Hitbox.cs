using System;
using UnityEngine;

internal class Box
{
    private readonly Vector3 size;
    private readonly Func<Vector3> position;

    private Vector3 Position => position();

    public Box(Func<Vector3> position, Vector3 size)
    {
        this.position = position;
        this.size = size;
    }

    private static bool IsBetween(float min1, float max1, float min2, float max2)
    {
        return max1 >= min2 && max2 >= min1;
    }

    public bool OverlapBox(Box other)
    {
        return IsBetween(GetLeft(), GetRight(), other.GetLeft(), other.GetRight()) &&
               IsBetween(GetBottom(), GetTop(), other.GetBottom(), other.GetTop()) &&
               IsBetween(GetFar(), GetNear(), other.GetFar(), other.GetNear());
    }

    public bool IsInside(Vector3 point)
    {
        return (point.x >= GetLeft() && point.x <= GetRight())
               && (point.y >= GetBottom() && point.y <= GetTop())
               && (point.z >= GetFar() && point.z <= GetNear());
    }

    public Vector3 GetIntersectionCenter(Box other)
    {
        var left = Mathf.Max(GetLeft(), other.GetLeft());
        var right = Mathf.Min(GetRight(), other.GetRight());
        var bottom = Mathf.Max(GetBottom(), other.GetBottom());
        var top = Mathf.Min(GetTop(), other.GetTop());
        var far = Mathf.Max(GetFar(), other.GetFar());
        var near = Mathf.Min(GetNear(), other.GetNear());
        return (new Vector3(left, bottom, far) + new Vector3(right, top, near)) / 2;
    }

    private float GetLeft()
    {
        return Position.x - size.x / 2;
    }

    private float GetRight()
    {
        return Position.x + size.x / 2;
    }

    private float GetBottom()
    {
        return Position.y - size.y / 2;
    }

    private float GetTop()
    {
        return Position.y + size.y / 2;
    }

    private float GetFar()
    {
        return Position.z - size.z / 2;
    }

    private float GetNear()
    {
        return Position.z + size.z / 2;
    }
}

/// <summary>
/// Hitbox for an attack, particle, or an object that can get hit
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(GameEntity))]
public class Hitbox : EntityBehaviour
{
    /// <value>
    /// Hitbox size
    /// </value>
    public Vector3 size;

    /// <value>
    /// Hitbox center point in world coordinates
    /// </value>
    public Vector3 WorldPosition => Entity.WorldPosition;

    private Box box;

    protected override void Awake()
    {
        base.Awake();
        box = new Box(() => WorldPosition, size);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Entity) return;
        Color lineColor = new(1.0f, 0.5f, 0.0f);
        Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
        DrawHitbox(lineColor, fillColor);
    }

    private void DrawHitbox(Color lineColor, Color fillColor)
    {
        DrawOutline(GameEntity.ScreenCoordinates(WorldPosition + size.z / 2 * Vector3.back),
            GameEntity.ScreenCoordinates(size + size.z * Vector3.back), lineColor);
        DrawBoth(GameEntity.ScreenCoordinates(WorldPosition + size.y / 2 * Vector3.up),
            GameEntity.ScreenCoordinates(size + size.y * Vector3.down), lineColor, fillColor);
        DrawFill(GameEntity.ScreenCoordinates(WorldPosition + WorldPosition.y * Vector3.down),
            GameEntity.ScreenCoordinates(size + size.y * Vector3.down), new Color(0, 0, 0, 0.5f));
    }

    private static void DrawBoth(Vector2 screenCenter, Vector2 screenSize, Color lineColor, Color fillColor)
    {
        DrawOutline(screenCenter, screenSize, lineColor);
        DrawFill(screenCenter, screenSize, fillColor);
    }

    private static void DrawOutline(Vector2 screenCenter, Vector2 screenSize, Color lineColor)
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireCube((Vector3)screenCenter + 0.01f * Vector3.forward,
            (Vector3)screenSize + 0.01f * Vector3.forward);
    }

    private static void DrawFill(Vector2 screenCenter, Vector2 screenSize, Color fillColor)
    {
        Gizmos.color = fillColor;
        Gizmos.DrawCube((Vector3)screenCenter + 0.01f * Vector3.forward, (Vector3)screenSize + 0.01f * Vector3.forward);
    }

    /// <summary>
    /// Checks if a point is inside the hitbox
    /// </summary>
    public bool IsInside(Vector3 point)
    {
        return box.IsInside(point);
    }

    /// <summary>
    /// Checks if this hitbox intersects another hitbox  
    /// </summary>
    public bool OverlapHitbox(Hitbox other)
    {
        return box.OverlapBox(other.box);
    }

    /// <summary>
    /// Finds the middle point in the intersection volume between two hitboxes
    /// </summary>
    public Vector3 GetIntersectionCenter(Hitbox other)
    {
        return box.GetIntersectionCenter(other.box);
    }
}