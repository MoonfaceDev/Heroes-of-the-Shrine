using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GameEntity : BaseComponent
{
    public GameEntity parent;
    public Tags tags;

    public Vector3 position = Vector3.zero;
    public Rotation rotation = Rotation.Right;
    public Vector3 scale = Vector3.one;

    private const float ZScale = 0.8f;

    public Vector3 WorldPosition
    {
        get => parent ? parent.TransformToWorld(position) : position;
        set => position = parent ? parent.TransformToRelative(value) : value;
    }

    public Rotation WorldRotation => parent ? (parent.WorldRotation * rotation) : rotation;

    public Vector3 WorldScale => parent ? Vector3.Scale(parent.WorldScale, scale) : scale;

    public Vector3 GroundWorldPosition => WorldPosition - WorldPosition.y * Vector3.up;

    protected virtual void Awake()
    {
        UpdateTransform();
        if (Application.isPlaying)
        {
            EntityManager.Instance.AddEntity(tags, this);
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            EntityManager.Instance.RemoveEntity(tags, this);
        }
    }

    protected void UpdateTransform()
    {
        transform.localPosition = GroundScreenCoordinates(position);
        transform.localRotation = rotation;
        transform.localScale = scale;
    }

    public Vector3 TransformToWorld(Vector3 relativePoint)
    {
        return WorldPosition + Vector3.Scale(WorldRotation * relativePoint, WorldScale);
    }

    public Vector3 TransformToRelative(Vector3 worldPoint)
    {
        var inverseWorldScale = new Vector3(1 / WorldScale.x, 1 / WorldScale.y, 1 / WorldScale.z);
        return Vector3.Scale(worldPoint - WorldPosition, -WorldRotation * inverseWorldScale);
    }

    public int SortingOrder => -1 * Mathf.RoundToInt(WorldPosition.z * 100f) * 10;

    public float GroundDistance(Vector3 point)
    {
        var distance = WorldPosition - point;
        distance.y = 0;
        return distance.magnitude;
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