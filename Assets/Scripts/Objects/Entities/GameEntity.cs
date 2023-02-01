using UnityEngine;

/// <summary>
/// The well known and loved <see cref="GameEntity"/> is a bridge between Heroes of the Shrine's coordinates system,
/// that has 3 (!) axes, to Unity's <see cref="Transform"/> that has only 2
/// </summary>
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GameEntity : BaseComponent
{
    /// <value>
    /// Parent <see cref="GameEntity"/>. Leave null if the object has no parents that are <see cref="GameEntity"/>.
    /// </value>
    public GameEntity parent;
    
    /// <value>
    /// Tags of the entity, multiple tags are allowed
    /// </value>
    public Tags tags;

    /// <value>
    /// Local position of the entity, relative to <see cref="parent"/>
    /// </value>
    public Vector3 position = Vector3.zero;
    
    /// <value>
    /// Local rotation of the entity, relative to <see cref="parent"/>
    /// </value>
    public Rotation rotation = Rotation.Right;
    
    /// <value>
    /// Local scale of the entity, relative to <see cref="parent"/>
    /// </value>
    public Vector3 scale = Vector3.one;

    private const float ZScale = 0.8f;

    /// <summary>
    /// Absolute position in world coordinates
    /// </summary>
    public Vector3 WorldPosition
    {
        get => parent ? parent.TransformToWorld(position) : position;
        set => position = parent ? parent.TransformToRelative(value) : value;
    }

    /// <summary>
    /// Absolute rotation
    /// </summary>
    public Rotation WorldRotation => parent ? parent.WorldRotation * rotation : rotation;

    /// <summary>
    /// Absolute scale
    /// </summary>
    public Vector3 WorldScale => parent ? Vector3.Scale(parent.WorldScale, scale) : scale;

    /// <summary>
    /// Absolute position in world coordinates, with <c>y=0</c>
    /// </summary>
    public Vector3 GroundWorldPosition => WorldPosition - WorldPosition.y * Vector3.up;

    protected virtual void Awake()
    {
        UpdateTransform();
        if (Application.isPlaying)
        {
            EntityManager.Instance.AddEntity(this);
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            EntityManager.Instance.RemoveEntity(this);
        }
    }

    protected void UpdateTransform()
    {
        transform.position = GroundScreenCoordinates(WorldPosition);
        transform.localRotation = rotation;
        transform.localScale = scale;
    }

    /// <summary>
    /// Transforms a relative coordinate to an absolute one
    /// </summary>
    /// <param name="relativePoint">Point relative to this entity</param>
    public Vector3 TransformToWorld(Vector3 relativePoint)
    {
        return WorldPosition + Vector3.Scale(WorldRotation * relativePoint, WorldScale);
    }

    /// <summary>
    /// Transforms an absolute coordinate to a relative one
    /// </summary>
    /// <param name="worldPoint">Absolute point</param>
    public Vector3 TransformToRelative(Vector3 worldPoint)
    {
        var inverseWorldScale = new Vector3(1 / WorldScale.x, 1 / WorldScale.y, 1 / WorldScale.z);
        return Vector3.Scale(worldPoint - WorldPosition, -WorldRotation * inverseWorldScale);
    }

    /// <value>
    /// Sorting order that related <see cref="Renderer"/> objects should have. Based on the Z position. 
    /// </value>
    public int SortingOrder => -1 * Mathf.RoundToInt(WorldPosition.z * 100f) * 10;

    /// <summary>
    /// Distance a point, excluding the Y distance
    /// </summary>
    /// <param name="point">World point to measure distance from</param>
    public float GroundDistance(Vector3 point)
    {
        var distance = WorldPosition - point;
        distance.y = 0;
        return distance.magnitude;
    }

    /// <summary>
    /// Converts a game point to the point where is rendered on the screen, without entity elevation (game's Y position)
    /// </summary>
    /// <param name="v">World position of the point, in game coordinates</param>
    public static Vector3 GroundScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.z * ZScale, 0);
    }

    /// <summary>
    /// Converts a game point to the point where is rendered on the screen, including elevation
    /// </summary>
    /// <param name="v">World position of the point, in game coordinates</param>
    public static Vector3 ScreenCoordinates(Vector3 v)
    {
        return new Vector3(v.x, v.y + v.z * ZScale, 0);
    }
}