using UnityEngine;

public class RectTrigger : BaseTrigger
{
    public Vector3 position;
    public Vector3 size;

    private MovableObject playerMovableObject;
    private bool fired = false;

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
        if (playerMovableObject && IsInside(playerMovableObject.position) && !fired)
        {
            fired = true;
            action.Invoke();
        }
    }
    
    private bool IsInside(Vector3 point)
    {
        return point.x > position.x && point.x < position.x + size.x
            && point.z > position.z && point.z < position.z + size.z;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(MovableObject.GroundScreenCoordinates(position + size/2), MovableObject.GroundScreenCoordinates(size));
    }
}
