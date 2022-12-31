using UnityEngine;
using UnityEngine.Serialization;

public class DeathAction : MonoBehaviour
{
    public float cameraZoomFactor;
    [FormerlySerializedAs("objctsToDestory")] public GameObject[] objectsToDestroy;
    public GameObject deathPanel;
    public float deathPanelTransitionDelay;

    public void Invoke()
    {
        foreach (var enemyBrain in FindObjectsOfType<EnemyBrain>())
        {
            Destroy(enemyBrain.gameObject);
        }

        foreach (var encounter in FindObjectsOfType<EncounterAction>())
        {
            encounter.Stop();
        }

        foreach (var @object in objectsToDestroy)
        {
            Destroy(@object);
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        var camera = Camera.main;

        if (camera)
        {
            var cameraFollow = camera.GetComponent<CameraFollow>();
            cameraFollow.enabled = false;
            var cameraFocus = camera.GetComponent<CameraFocus>();
            cameraFocus.Zoom(player.transform.position, cameraZoomFactor);
        }

        EventManager.Instance.StartTimeout(() => deathPanel.SetActive(true), deathPanelTransitionDelay);
    }
}
