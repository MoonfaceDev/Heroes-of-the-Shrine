using UnityEngine;

public class CameraFollow : BaseComponent
{
    public string targetTag;

    private CameraMovement cameraMovement;
    private MovableObject target;
    private Vector3 offset;

    private void Awake()
    {
        cameraMovement = GetComponent<CameraMovement>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();
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
}