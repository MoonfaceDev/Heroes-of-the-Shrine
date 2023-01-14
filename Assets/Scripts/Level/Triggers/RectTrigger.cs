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

    protected override void Update()
    {
        base.Update();
        if (playerMovableObject && IsInside(playerMovableObject.WorldPosition) && !fired)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(GameEntity.GroundScreenCoordinates(position + size / 2),
            GameEntity.GroundScreenCoordinates(size));
    }
}