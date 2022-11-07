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
    public Vector3 WorldSize => Vector3.Scale(movableObject.parent.rotation * size, movableObject.parent.scale);

    public void PlayParticles()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem)
        {
            particleSystem.Play();
        }
    }

    void OnDrawGizmos()
    {
        if (CompareTag("Barrier"))
        {
            Color lineColor = new(1.0f, 0.5f, 0.0f);
            Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
            DrawOutline(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetBottom(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, size.y, 0))), lineColor);
            DrawBoth(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetTop(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, 0, size.z))), lineColor, fillColor);
            DrawFill(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), 0, GetFar())), MovableObject.ScreenCoordinates(new(size.x, 0, size.z))), new Color(0, 0, 0, 0.5f));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!CompareTag("Barrier"))
        {
            Color lineColor = new(1.0f, 0.5f, 0.0f);
            Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
            DrawOutline(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetBottom(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, size.y, 0))), lineColor);
            DrawBoth(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetTop(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, 0, size.z))), lineColor, fillColor);
            DrawFill(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), 0, GetFar())), MovableObject.ScreenCoordinates(new(size.x, 0, size.z))), new Color(0, 0, 0, 0.5f));
        }
    }

    void DrawBoth(Rect rect, Color lineColor, Color fillColor)
    {
        DrawOutline(rect, lineColor);
        DrawFill(rect, fillColor);
    }

    void DrawOutline(Rect rect, Color lineColor)
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    void DrawFill(Rect rect, Color fillColor)
    {
        Gizmos.color = fillColor;
        Gizmos.DrawCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    public bool IsInside(Vector3 point)
    {
        return point.x >= GetLeft() && point.x <= GetRight()
            && point.y >= GetBottom() && point.y <= GetTop()
            && point.z >= GetFar() && point.z <= GetNear();
    }

    static bool IsBetween(float min1, float max1, float min2, float max2)
    {
        return max1 >= min2 && max2 >= min1;
    }

    public bool OverlapHitbox(Hitbox hitbox, float padding)
    {
        return IsBetween(GetLeft() + padding, GetRight() - padding, hitbox.GetLeft() + padding, hitbox.GetRight() - padding) &&
            IsBetween(GetBottom() + padding, GetTop() - padding, hitbox.GetBottom() + padding, hitbox.GetTop() - padding) &&
            IsBetween(GetFar() + padding, GetNear() - padding, hitbox.GetFar() + padding, hitbox.GetNear() - padding);
    }

    public List<Vector2> GetSegmentIntersections(Vector2 start, Vector2 end)
    {
        return LineRectangleIntersections(start, end, ToPlane(WorldPosition), ToPlane(size));
    }

    public float GetLeft()
    {
        return Mathf.Min(WorldPosition.x, WorldPosition.x + WorldSize.x);
    }

    public float GetRight()
    {
        return Mathf.Max(WorldPosition.x, WorldPosition.x + WorldSize.x);
    }

    public float GetBottom()
    {
        return Mathf.Min(WorldPosition.y, WorldPosition.y + WorldSize.y);
    }
    public float GetTop()
    {
        return Mathf.Max(WorldPosition.y, WorldPosition.y + WorldSize.y);
    }

    public float GetFar()
    {
        return Mathf.Min(WorldPosition.z, WorldPosition.z + WorldSize.z);
    }

    public float GetNear()
    {
        return Mathf.Max(WorldPosition.z, WorldPosition.z + WorldSize.z);
    }
}

