using System;
using UnityEngine;

[RequireComponent(typeof(Pathfind))]
[RequireComponent(typeof(WalkBehaviour))]
public class FollowBehaviour : CharacterBehaviour
{
    public event Action onStart;
    public event Action onStop;

    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
        }
    }

    private bool _active;
    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;
    private EventListener followEvent;

    public override void Awake()
    {
        base.Awake();
        pathfind = GetComponent<Pathfind>();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Follow(MovableObject target, float speedMultiplier)
    {
        active = true;
        onStart?.Invoke();

        walkBehaviour.speed = walkBehaviour.defaultSpeed * speedMultiplier;

        followEvent = eventManager.Attach(() => true, () => {
            Vector3 direction = pathfind.Direction(movableObject.position, target.position);
            walkBehaviour.Walk(direction.x, direction.z);
        }, false);
    }

    public void Stop()
    {
        active = false;
        onStop?.Invoke();

        eventManager.Detach(followEvent);
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
        walkBehaviour.Stop(true);
    }
}