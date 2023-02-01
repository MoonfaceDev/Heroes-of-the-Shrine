using ExtEvents;
using UnityEngine;

/// <value>
/// Invokes an event when player enters a rectangle for the first time
/// </value>
public class RectTrigger : BaseComponent
{
    /// <value>
    /// Position of the left-bottom-near point of the cube 
    /// </value>
    public Vector3 position;
    
    /// <value>
    /// Size of the cube
    /// </value>
    public Vector3 size;
    
    /// <value>
    /// Target event
    /// </value>
    [SerializeField] public ExtEvent action;

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