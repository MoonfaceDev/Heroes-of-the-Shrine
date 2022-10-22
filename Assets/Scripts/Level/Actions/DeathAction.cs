using UnityEngine;

public class DeathAction : BaseAction
{
    public float cameraZoomFactor;
    public GameObject[] objctsToDestory;
    public GameObject deathPanel;
    public float deathPanelTransitionDelay;

    public override void Invoke()
    {
        foreach (EnemyBrain enemyBrain in FindObjectsOfType<EnemyBrain>())
        {
            Destroy(enemyBrain.gameObject);
        }

        foreach (EncounterAction encounter in FindObjectsOfType<EncounterAction>())
        {
            encounter.Stop();
        }

        foreach (GameObject @object in objctsToDestory)
        {
            Destroy(@object);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Camera camera = Camera.main;

        CameraFollow cameraFollow = camera.GetComponent<CameraFollow>();
        cameraFollow.enabled = false;
        CameraFocus cameraFocus = camera.GetComponent<CameraFocus>();
        cameraFocus.Zoom((Vector2)player.transform.position + 1f * Vector2.up, cameraZoomFactor);

        EventManager.Instance.StartTimeout(() => deathPanel.SetActive(true), deathPanelTransitionDelay);
    }
}
