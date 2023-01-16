using UnityEngine;

public class RectTrigger : BaseTrigger
{
    public Vector3 position;
    public Vector3 size;

    private GameEntity player;
    private bool fired;

    private void Awake()
    {
        player = EntityManager.Instance.GetEntity(Tag.Player);
    }

    protected override void Update()
    {
        base.Update();
        if (player && IsInside(player.WorldPosition) && !fired)
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