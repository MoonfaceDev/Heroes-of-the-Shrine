using UnityEngine;

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : SoloMovementBehaviour
{
    public bool Active
    {
        get => active;
        private set => active = value;
    }

    public override bool Playing => Active;

    private bool active;
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
        walkBehaviour.OnStop += Stop;
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

        followEvent = EventManager.Attach(() => true, () => {
            Vector3 direction = pathfind.Direction(MovableObject.position, target.position);
            walkBehaviour.Play(direction.x, direction.z);
        }, false);
    }

    public override void Stop()
    {
        if (Active)
        {
            InvokeOnStop();
            Active = false;
            EventManager.Detach(followEvent);
            walkBehaviour.speed.RemoveModifier(speedModifier);
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        }
    }
}