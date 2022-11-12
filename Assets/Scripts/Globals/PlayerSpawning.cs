using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawning : MonoBehaviour
{
    public UnityEvent startEvent;

    private void Awake()
    {
        SetupCamera();
    }

    private static void SetupCamera()
    {
        var camera = Camera.main;
        if (camera == null) return;
        var cameraMovement = camera.GetComponent<CameraMovement>();
        var position = camera.transform.position;
        position += (cameraMovement.worldBorder.xMin + cameraMovement.CameraWidth / 2 - position.x) * Vector3.right;
        camera.transform.position = position;
    }

    private void Start()
    {
        startEvent.Invoke();
    }
}
