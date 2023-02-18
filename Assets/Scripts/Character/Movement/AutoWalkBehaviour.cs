using System;
using UnityEngine;

public class AutoWalkBehaviour : PlayableBehaviour<AutoWalkBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
        public Vector3 destination;
        public Func<Node[]> getExcluded = null;
    }

    public override bool Playing => active;

    private bool active;
    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;
    private string followListener;

    protected override void Awake()
    {
        base.Awake();
        pathfind = GetBehaviour<Pathfind>();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
    }

    protected override void DoPlay(Command command)
    {
        active = true;

        followListener = Register(() =>
        {
            var direction = pathfind.Direction(MovableEntity.WorldPosition, command.destination,
                command.getExcluded?.Invoke());
            walkBehaviour.Play(new WalkBehaviour.Command(direction, false));
            MovableEntity.WorldRotation =
                Mathf.RoundToInt(Mathf.Sign(command.destination.x - MovableEntity.WorldPosition.x));
        });
    }

    protected override void DoStop()
    {
        active = false;
        Unregister(followListener);
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}