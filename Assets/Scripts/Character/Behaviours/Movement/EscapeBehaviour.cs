using System;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : SoloMovementBehaviour
{
    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
        }
    }

    public override bool Playing => active;

    private bool _active;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener escapeEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        movableObject.onStuck += Stop;
        walkBehaviour.onStop += Stop;
    }

    public void Play(MovableObject target, float speedMultiplier, bool fitLookDirection = true)
    {
        if (!CanPlay())
        {
            return;
        }
        active = true;
        InvokeOnPlay();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        escapeEvent = eventManager.Attach(() => true, () => {
            Vector3 distance = movableObject.position - target.position;
            distance.y = 0;
            Vector3 direction = distance.normalized;
            walkBehaviour.Play(direction.x, direction.z, fitLookDirection);
            lookDirection = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        }, false);
    }

    public override void Stop()
    {
        if (active)
        {
            InvokeOnStop();
            active = false;
            eventManager.Detach(escapeEvent);
            walkBehaviour.speed.RemoveModifier(speedModifier);
            StopBehaviours(typeof(WalkBehaviour));
            movableObject.velocity = Vector3.zero;
        }
    }
}
