using System;
using UnityEngine;

public delegate bool GetOverrideDirection(out Vector3 direction);

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : BaseMovementBehaviour<FollowBehaviour.Command>
{
    public class Command
    {
        public readonly MovableEntity target;
        public readonly Func<Node[]> getExcluded;
        public readonly GetOverrideDirection getOverrideDirection;

        public Command(MovableEntity target, Func<Node[]> getExcluded = null,
            GetOverrideDirection getOverrideDirection = null)
        {
            this.target = target;
            this.getExcluded = getExcluded;
            this.getOverrideDirection = getOverrideDirection;
        }
    }
    
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

    protected override void DoPlay(Command command)
    {
        active = true;

        followListener = Register(() =>
        {
            var direction = GetDirection(command.target, command.getExcluded?.Invoke(), command.getOverrideDirection);
            walkBehaviour.Play(new WalkBehaviour.Command(direction, false));
            MovableEntity.rotation =
                Mathf.RoundToInt(Mathf.Sign(command.target.WorldPosition.x - MovableEntity.WorldPosition.x));
        });
    }

    private Vector3 GetDirection(MovableEntity target, Node[] excluded = null,
        GetOverrideDirection getOverrideDirection = null)
    {
        if (getOverrideDirection != null)
        {
            var shouldOverride = getOverrideDirection(out var direction);
            if (shouldOverride)
            {
                return direction;
            }
        }

        return pathfind.Direction(MovableEntity.WorldPosition, target.WorldPosition, excluded);
    }

    protected override void DoStop()
    {
        active = false;
        Unregister(followListener);
        walkBehaviour.Stop();
    }
}