﻿using System;
using UnityEngine;

public class AutoWalkBehaviour : BaseMovementBehaviour<AutoWalkBehaviour.Command>
{
    public class Command
    {
        public readonly Vector3 destination;
        public readonly Func<Node[]> getExcluded;

        public Command(Vector3 destination, Func<Node[]> getExcluded = null)
        {
            this.destination = destination;
            this.getExcluded = getExcluded;
        }
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