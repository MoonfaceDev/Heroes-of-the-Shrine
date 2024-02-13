using ExtEvents;
using UnityEngine;

/// <value>
/// Invokes an event when player crosses a line (X value) for the first time
/// </value>
public class LineTrigger : BaseComponent
{
    /// <value>
    /// X value that player has to cross
    /// </value>
    public float minimumX;
    
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
        if (player && player.WorldPosition.x >= minimumX && !fired)
        {
            fired = true;
            action.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Vector3(minimumX, 0, 0.01f), Vector3.up * 10);
        Gizmos.DrawRay(new Vector3(minimumX, 0, 0.01f), Vector3.down * 10);
    }
}