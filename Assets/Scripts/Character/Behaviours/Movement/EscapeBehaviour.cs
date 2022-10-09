using System;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : CharacterBehaviour
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
    private WalkBehaviour walkBehaviour;
    private EventListener escapeEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Escape(MovableObject target, float speedMultiplier)
    {
        active = true;
        onStart?.Invoke();

        walkBehaviour.speed = walkBehaviour.defaultSpeed * speedMultiplier;

        movableObject.onStuck += Stop;

        escapeEvent = eventManager.Attach(() => true, () => {
            Vector3 distance = movableObject.position - target.position;
            distance.y = 0;
            Vector3 direction = distance.normalized;
            walkBehaviour.Walk(direction.x, direction.z);
        }, false);
    }

    public void Stop()
    {
        active = false;
        onStop?.Invoke();

        movableObject.onStuck -= Stop;
        eventManager.Detach(escapeEvent);
        walkBehaviour.Stop(true);
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
    }
}
