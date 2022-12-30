using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public Vector3 target;

    public void Invoke()
    {
        cameraMovement.targetPosition = target;
    }
}