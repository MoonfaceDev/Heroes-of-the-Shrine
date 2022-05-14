using UnityEngine;

public class BarrierRectangle : MovableObject
{

    public Vector2 size;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        DrawRect(new Rect(position, size));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        DrawRect(new Rect(position, size));
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    public float GetLeft()
    {
        return position.x;
    }

    public float GetRight()
    {
        return position.x + size.x;
    }

    public float GetTop()
    {
        return position.y + size.y;
    }

    public float GetBottom()
    {
        return position.y;
    }


    public bool IsInside(Vector3 position)
    {
        return position.x > GetLeft() && position.x < GetRight() && position.z > GetBottom() / zScale && position.z < GetTop() / zScale;
    }

}
