using UnityEngine;

public class CameraFollow : MonoBehaviour
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
        if (target)
        {
            offset = transform.position - MovableObject.GroundScreenCoordinates(target.WorldPosition);
            offset.x = 0;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = MovableObject.GroundScreenCoordinates(target.WorldPosition) + offset;
        cameraMovement.Move(targetPosition);
    }
}