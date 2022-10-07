using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MathUtils;

[ExecuteInEditMode]
public class Hitbox : MonoBehaviour
{
    public Vector3 position;
    public Vector3 size;
    public MovableObject parentObject;

    void OnDrawGizmosSelected()
    {
        if (parentObject != null)
        {
            Color lineColor = new(1.0f, 0.5f, 0.0f);
            Color fillColor = new(1.0f, 0.5f, 0.0f, 0.3f);
            DrawRect(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetBottom(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, size.y, 0))), false, lineColor, Color.white);
            DrawRect(new Rect(MovableObject.ScreenCoordinates(new(GetLeft(), GetTop(), GetFar())), MovableObject.ScreenCoordinates(new(size.x, 0, size.z))), true, lineColor, fillColor);
        }
    }

    void DrawRect(Rect rect, bool filled, Color lineColor, Color fillColor)
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
        if (filled)
        {
            Gizmos.color = fillColor;
            Gizmos.DrawCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
        }
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

    public List<Vector3> GetSegmentIntersections(Vector3 start, Vector3 end, float offset = 0)
    {
        return LineRectangleIntersections(ToPlane(start), ToPlane(end), ToPlane(position), ToPlane(size), offset).Select(point => ToSpace(point)).ToList();
    }

    public Vector3 WorldPosition
    {
        get => parentObject.position + new Vector3(
            position.x * (1 - 2 * Mathf.Pow(parentObject.transform.rotation.y, 2)), 
            position.y, 
            position.z
        );
    }

    public float GetLeft()
    {
        if (1 - 2 * Mathf.Pow(parentObject.transform.rotation.y, 2) > 0)
        {
            return WorldPosition.x;
        }
        return WorldPosition.x - size.x;
    }

    public float GetRight()
    {
        if (1 - 2 * Mathf.Pow(parentObject.transform.rotation.y, 2) < 0)
        {
            return WorldPosition.x;
        }
        return WorldPosition.x + size.x;
    }

    public float GetTop()
    {
        return WorldPosition.y + size.y;
    }

    public float GetBottom()
    {
        return WorldPosition.y;
    }

    public float GetNear()
    {
        return WorldPosition.z + size.z;
    }
    public float GetFar()
    {
        return WorldPosition.z;
    }
}

