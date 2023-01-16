using UnityEngine;

public class LineTrigger : BaseTrigger
{
    public float minimumX;

    private GameEntity player;
    private bool fired;

    private void Awake()
    {
        player = EntityManager.Instance.GetEntity(Tag.Player);
    }

    protected override void Update()
    {
        base.Update();
        if (player && player.WorldPosition.x >= minimumX && !fired)
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