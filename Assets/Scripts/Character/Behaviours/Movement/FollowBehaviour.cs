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
    private string followListener;

    public override void Awake()
    {
        base.Awake();
        pathfind = GetComponent<Pathfind>();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Play(Vector3 destination, Func<Node[]> getExcluded = null)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        onPlay.Invoke();
        
        followListener = Register(() => {
            var direction = pathfind.Direction(MovableObject.WorldPosition, destination, getExcluded?.Invoke());
            walkBehaviour.Play(direction.x, direction.z, false);
            MovableObject.rotation = Mathf.RoundToInt(Mathf.Sign(destination.x - MovableObject.WorldPosition.x));
        });
    }

    public void Play(MovableObject target, Func<Node[]> getExcluded = null, GetOverrideDirection getOverrideDirection = null)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        onPlay.Invoke();

        followListener = Register(() => {
            var direction = GetDirection(target, getExcluded?.Invoke(), getOverrideDirection);
            walkBehaviour.Play(direction.x, direction.z, false);
            MovableObject.rotation = Mathf.RoundToInt(Mathf.Sign(target.WorldPosition.x - MovableObject.WorldPosition.x));
        });
    }

    private Vector3 GetDirection(MovableObject target, Node[] excluded = null, GetOverrideDirection getOverrideDirection = null)
    {
        if (getOverrideDirection != null)
        {
            var shouldOverride = getOverrideDirection(out Vector3 direction);
            if (shouldOverride)
            {
                return direction;
            }
        }
        return pathfind.Direction(MovableObject.WorldPosition, target.WorldPosition, excluded);
    }

    public override void Stop()
    {
        if (!active) return;
        onStop.Invoke();
        active = false;
        Unregister(followListener);
        StopBehaviours(typeof(WalkBehaviour));
        MovableObject.velocity = Vector3.zero;
    }
}