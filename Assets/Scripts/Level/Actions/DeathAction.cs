using UnityEngine;

public class DeathAction : BaseComponent
{
    public float cameraZoomFactor;
    public GameObject[] objectsToDestroy;
    public GameObject deathPanel;
    public float deathPanelTransitionDelay;

    public void Invoke()
    {
        foreach (var enemy in EntityManager.Instance.GetEntities(Tag.Enemy))
        {
            Destroy(enemy.gameObject);
        }

        foreach (var encounter in FindObjectsOfType<EncounterAction>())
        {
            encounter.Stop();
        }

        foreach (var @object in objectsToDestroy)
        {
            Destroy(@object);
        }

        var player = EntityManager.Instance.GetEntity(Tag.Player);
        var camera = Camera.main;

        if (camera)
        {
            var cameraFollow = camera.GetComponent<CameraFollow>();
            cameraFollow.enabled = false;
            var cameraFocus = camera.GetComponent<CameraFocus>();
            cameraFocus.Zoom(player.transform.position, cameraZoomFactor);
        }

        StartTimeout(() => deathPanel.SetActive(true), deathPanelTransitionDelay);
    }
}