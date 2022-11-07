using System;
using UnityEngine;

public delegate bool GetOverrideDirection(out Vector3 direction);

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : BaseMovementBehaviour
{
    public override bool Playing => active;

    private bool active;
    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;
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

    public void Play(Vector3 destination, Func<Node[]> GetExcluded = null)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        InvokeOnPlay();
        followEvent = EventManager.Attach(() => true, () => {
            Vector3 direction = pathfind.Direction(MovableObject.WorldPosition, destination, GetExcluded != null ? GetExcluded() : null);
            walkBehaviour.Play(direction.x, direction.z, false);
            LookDirection = Mathf.RoundToInt(Mathf.Sign(destination.x - MovableObject.WorldPosition.x));
        }, false);
    }

    public void Play(MovableObject target, Func<Node[]> GetExcluded = null, GetOverrideDirection getOverrideDirection = null)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        InvokeOnPlay();

        followEvent = EventManager.Attach(() => true, () => {
            Vector3 direction = GetDirection(target, GetExcluded != null ? GetExcluded() : null, getOverrideDirection);
            walkBehaviour.Play(direction.x, direction.z, false);
            LookDirection = Mathf.RoundToInt(Mathf.Sign(target.WorldPosition.x - MovableObject.WorldPosition.x));
        }, false);
    }

    private Vector3 GetDirection(MovableObject target, Node[] excluded = null, GetOverrideDirection getOverrideDirection = null)
    {
        if (getOverrideDirection != null)
        {
            bool shouldOverride = getOverrideDirection(out Vector3 direction);
            if (shouldOverride)
            {
                return direction;
            }
        }
        return pathfind.Direction(MovableObject.WorldPosition, target.WorldPosition, excluded);
    }

    public override void Stop()
    {
        if (active)
        {
            InvokeOnStop();
            active = false;
            EventManager.Detach(followEvent);
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        }
    }
}