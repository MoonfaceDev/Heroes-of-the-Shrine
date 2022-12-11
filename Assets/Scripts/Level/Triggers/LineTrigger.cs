using UnityEngine;

public class LineTrigger : BaseTrigger
{
    public float minimumX;

    private MovableObject playerMovableObject;
    private bool fired;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerMovableObject = player.GetComponent<MovableObject>();
        }
    }

    private void Update()
    {
        if (playerMovableObject && playerMovableObject.WorldPosition.x >= minimumX && !fired)
        {
            fired = true;
            action.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Vector3(minimumX, 0, 0.01f), Vector3.up * 10);
        Gizmos.DrawRay(new Vector3(minimumX, 0, 0.01f), Vector3.down * 10);
    }
}
