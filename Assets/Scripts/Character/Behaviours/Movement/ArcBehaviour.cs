using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]

public class ArcBehaviour : BaseMovementBehaviour
{
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener circleEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        walkBehaviour.onStop.AddListener(Stop);
    }

    public void Play(MovableObject target, float speedMultiplier)
    {
        if (!CanPlay())
        {
            return;
        }
        Active = true;
        InvokeOnPlay();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        var playerPosition = target.WorldPosition;
        var initialDistance = walkBehaviour.MovableObject.WorldPosition - playerPosition;
        initialDistance.y = 0;
        var radius = initialDistance.magnitude;
        var clockwise = Mathf.Sign(Random.Range(-1f, 1f));

        circleEvent = EventManager.Attach(() => true, () => {
            var distance = walkBehaviour.MovableObject.WorldPosition - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            var newPosition = playerPosition + distance;
            newPosition.y = walkBehaviour.MovableObject.WorldPosition.y;
            walkBehaviour.MovableObject.WorldPosition = newPosition;
            var direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(direction.x, direction.z, false);
            if ((target.WorldPosition - walkBehaviour.MovableObject.WorldPosition).x != 0)
            {
                walkBehaviour.MovableObject.rotation = Mathf.RoundToInt(Mathf.Sign((target.WorldPosition - walkBehaviour.MovableObject.WorldPosition).x));
            }
        }, false);
    }

    public override void Stop()
    {
        if (!Active) return;
        InvokeOnStop();
        Active = false;
        EventManager.Detach(circleEvent);
        walkBehaviour.speed.RemoveModifier(speedModifier);
        StopBehaviours(typeof(WalkBehaviour));
        MovableObject.velocity = Vector3.zero;
    }
}
