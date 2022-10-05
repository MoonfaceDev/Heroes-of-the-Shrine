using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapePattern : BasePattern
{
    public MovableObject player;
    public float speedMultiplier;
    public float minDistance;

    private WalkBehaviour walkBehaviour;
    private EventListener escapeEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();

        walkBehaviour.speed = walkBehaviour.defaultSpeed * speedMultiplier;

        movableObject.onStuck += StopPattern;

        escapeEvent = eventManager.Attach(() => true, () => {
            Vector3 distance = movableObject.position - player.position;
            distance.y = 0;
            Vector3 direction = distance.normalized;
            walkBehaviour.Walk(direction.x, direction.z);
        }, false);

        eventManager.Attach(() =>
        {
            Vector3 distance = player.position - movableObject.position;
            distance.y = 0;
            return distance.magnitude < minDistance;
        }, StopPattern);
    }

    public override void StopPattern()
    {
        base.StopPattern();
        movableObject.onStuck -= StopPattern;
        eventManager.Detach(escapeEvent);
        walkBehaviour.Stop(true);
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
    }
}
