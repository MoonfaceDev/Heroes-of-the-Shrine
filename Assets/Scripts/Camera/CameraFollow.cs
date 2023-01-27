using UnityEngine;

public class CameraFollow : BaseComponent
{
    public Tag targetTag;

    private CameraMovement cameraMovement;
    private GameEntity target;
    private Vector3 offset;

    private void Awake()
    {
        cameraMovement = GetComponent<CameraMovement>();
    }

    private void Start()
    {
        target = EntityManager.Instance.GetEntity(targetTag);
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

    public void AddOffset(Vector3 change)
    {
        offset += change;
    }
}