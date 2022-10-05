using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class ArcPattern : BasePattern
{
    public MovableObject player;
    public float speedMultiplier;

    private WalkBehaviour walkBehaviour;
    private EventListener circleEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();

        walkBehaviour.speed = walkBehaviour.defaultSpeed * speedMultiplier;

        Vector3 playerPosition = player.position;

        Vector3 initialDistance = movableObject.position - playerPosition;
        initialDistance.y = 0;
        float radius = initialDistance.magnitude;

        float clockwise = Mathf.Sign(Random.Range(-1f ,1f));

        circleEvent = eventManager.Attach(() => true, () => {
            Vector3 distance = movableObject.position - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            movableObject.position = playerPosition + distance;
            Vector3 direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Walk(direction.x, direction.z, false);
            if ((player.position - movableObject.position).x != 0) {
                lookDirection = Mathf.RoundToInt(Mathf.Sign((player.position - movableObject.position).x));
            };
        }, false);

        movableObject.onStuck += StopPattern;
    }

    public override void StopPattern()
    {
        base.StopPattern();
        movableObject.onStuck -= StopPattern;
        eventManager.Detach(circleEvent);
        walkBehaviour.Stop(true);
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
    }
}
