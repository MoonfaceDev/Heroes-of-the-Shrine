using UnityEngine;

/// <summary>
/// Make camera follow an entity
/// </summary>
public class CameraFollow : BaseComponent
{
    /// <value>
    /// Tracked entity
    /// </value>
    public GameEntity target;

    private CameraMovement cameraMovement;
    private Vector3 offset;

    private void Awake()
    {
        cameraMovement = GetComponent<CameraMovement>();
    }

    private void Start()
    {
        if (!target) return;
        offset = transform.position - GameEntity.GroundScreenCoordinates(target.WorldPosition);
        offset.x = 0;
    }

    private void LateUpdate()
    {
        if (!target) return;
        var targetPosition = GameEntity.GroundScreenCoordinates(target.WorldPosition) + offset;
        cameraMovement.targetPosition = targetPosition;
    }

    /// <summary>
    /// Modify the distance between camera and the tracked object
    /// </summary>
    /// <param name="change">Offset vector to add. <see cref="Vector3.zero"/> will leave it the same.</param>
    public void AddOffset(Vector3 change)
    {
        offset += change;
    }
}