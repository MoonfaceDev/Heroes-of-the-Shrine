using System;
using UnityEngine;

public class AutoWalkCommand : ICommand
{
    public readonly Vector3 destination;
    public readonly Func<Node[]> getExcluded;

    public AutoWalkCommand(Vector3 destination, Func<Node[]> getExcluded = null)
    {
        this.destination = destination;
        this.getExcluded = getExcluded;
    }
}

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class AutoWalkBehaviour : BaseMovementBehaviour<AutoWalkCommand>
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

    protected override void DoPlay(AutoWalkCommand command)
    {
        active = true;

        followListener = Register(() =>
        {
            var direction = pathfind.Direction(MovableEntity.WorldPosition, command.destination,
                command.getExcluded?.Invoke());
            walkBehaviour.Play(new WalkCommand(direction.x, direction.z, false));
            MovableEntity.rotation =
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