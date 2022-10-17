using UnityEngine;

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : SoloMovementBehaviour
{
    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
        }
    }

    public override bool Playing => active;

    private bool _active;
    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener followEvent;

    public override void Awake()
    {
        base.Awake();
        pathfind = GetComponent<Pathfind>();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        walkBehaviour.onStop += Stop;
    }

    public void Play(MovableObject target, float speedMultiplier)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        InvokeOnPlay();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        followEvent = eventManager.Attach(() => true, () => {
            Vector3 direction = pathfind.Direction(movableObject.position, target.position);
            walkBehaviour.Play(direction.x, direction.z);
        }, false);
    }

    public override void Stop()
    {
        if (active)
        {
            InvokeOnStop();
            active = false;
            eventManager.Detach(followEvent);
            walkBehaviour.speed.RemoveModifier(speedModifier);
            StopBehaviours(typeof(WalkBehaviour));
            movableObject.velocity = Vector3.zero;
        }
    }
}