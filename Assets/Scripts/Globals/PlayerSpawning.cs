using UnityEngine;

public class PlayerSpawning : MonoBehaviour
{
    public GameObject playerPrefab;
    public float entranceDuration;
    public float entranceSpeedMultiplier = 1;

    void Awake()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.FadeIn();

        Camera camera = SetupCamera();
        CameraFollow cameraFollow = camera.GetComponent<CameraFollow>();
        SpawnPlayer(cameraFollow.worldBorder.xMin + 0.5f);
    }

    private void Start()
    {
        PlayEntrance();
    }

    private GameObject SpawnPlayer(float x)
    {
        GameObject player = Instantiate(playerPrefab);
        MovableObject movableObject = player.GetComponent<MovableObject>();
        movableObject.position.x = x;
        return player;
    }

    private Camera SetupCamera()
    {
        Camera camera = Camera.main;
        CameraFollow cameraFollow = camera.GetComponent<CameraFollow>();
        camera.transform.position += (cameraFollow.worldBorder.xMin + cameraFollow.CameraWidth / 2 - camera.transform.position.x) * Vector3.right;
        return camera;
    }

    private void PlayEntrance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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
    }
}
