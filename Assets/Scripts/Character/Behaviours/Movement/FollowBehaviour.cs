using UnityEngine;

public delegate bool GetOverrideDirection(out Vector3 direction);

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : BaseMovementBehaviour
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

    public void Play(MovableObject target, float speedMultiplier, IsExcluded isExcluded = null, GetOverrideDirection getOverrideDirection = null)
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
            Vector3 direction = GetDirection(target, isExcluded, getOverrideDirection);
            walkBehaviour.Play(direction.x, direction.z, false);
            LookDirection = Mathf.RoundToInt(Mathf.Sign(target.position.x - MovableObject.position.x));
        }, false);
    }

    private Vector3 GetDirection(MovableObject target, IsExcluded isExcluded, GetOverrideDirection getOverrideDirection)
    {
        if (getOverrideDirection != null)
        {
            bool shouldOverride = getOverrideDirection(out Vector3 direction);
            if (shouldOverride)
            {
                return direction;
            }
        }
        return pathfind.Direction(MovableObject.position, target.position, isExcluded);
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