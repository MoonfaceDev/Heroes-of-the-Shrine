using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawning : MonoBehaviour
{
    public GameObject player;
    public Vector3 spawnPosition;
    // public float entranceDuration;
    // public float entranceSpeedMultiplier = 1;
    public UnityEvent startEvent;

    void Awake()
    {
        Camera camera = SetupCamera();
        // CameraMovement cameraMovement = camera.GetComponent<CameraMovement>();
        MovableObject movableObject = player.GetComponent<MovableObject>();
        // movableObject.position.x = cameraMovement.worldBorder.xMin + 0.5f;
        movableObject.position = spawnPosition;
    }

    private Camera SetupCamera()
    {
        Camera camera = Camera.main;
        CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
        camera.transform.position += (cameraMovement.worldBorder.xMin + cameraMovement.CameraWidth / 2 - camera.transform.position.x) * Vector3.right;
        return camera;
    }

    private void Start()
    {
        startEvent.Invoke();
        // PlayEntrance();
    }

    /*private void PlayEntrance()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        PlayerController controller = player.GetComponent<PlayerController>();
        WalkBehaviour walkBehaviour = player.GetComponent<WalkBehaviour>();

        controller.Enabled = false;
        cameraFollow.enabled = false;

        IModifier modifier = new MultiplierModifier(entranceSpeedMultiplier);
        walkBehaviour.speed.AddModifier(modifier);
        walkBehaviour.Play(1, 0);

        EventManager.Instance.StartTimeout(() =>
        {
            walkBehaviour.Stop();
            walkBehaviour.speed.RemoveModifier(modifier);
            controller.Enabled = true;
            cameraFollow.enabled = true;
        }, entranceDuration);
    }*/

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(spawnPosition), 0.5f);
    }
}
