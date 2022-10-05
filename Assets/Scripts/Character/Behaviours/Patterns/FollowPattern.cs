using UnityEngine;

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowPattern : BasePattern
{
    public MovableObject player;
    public float speedMultiplier;
    public float minimumDistance;

    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;
    private EventListener followEvent;

    public override void Awake()
    {
        base.Awake();
        pathfind = GetComponent<Pathfind>();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();
        walkBehaviour.speed = walkBehaviour.defaultSpeed * speedMultiplier;

        followEvent = eventManager.Attach(() => true, () => {
            Vector3 direction = pathfind.Direction(movableObject.position, player.position);
            walkBehaviour.Walk(direction.x, direction.z);
        }, false);

        eventManager.Attach(() =>
        {
            Vector3 distance = player.position - movableObject.position;
            distance.y = 0;
            return distance.magnitude < minimumDistance;
        }, StopPattern);
    }

    public override void StopPattern()
    {
        base.StopPattern();
        eventManager.Detach(followEvent);
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
        walkBehaviour.Stop(true);
    }
}
