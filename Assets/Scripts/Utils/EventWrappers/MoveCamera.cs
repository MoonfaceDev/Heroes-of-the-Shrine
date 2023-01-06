using UnityEngine;

public class MoveCamera : BaseComponent
{
    public CameraMovement cameraMovement;
    public Vector3 target;

    public void Invoke()
    {
        cameraMovement.targetPosition = target;
    }
}