using System;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : BaseMovementBehaviour
{
    public bool Active
    {
        get => active;
        private set => active = value;
    }

    public override bool Playing => Active;

    private bool active;
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
        MovableObject.OnStuck += Stop;
        walkBehaviour.OnStop += Stop;
    }

    public void Play(MovableObject target, float speedMultiplier, bool fitLookDirection = true)
    {
        if (!CanPlay())
        {
            return;
        }
        Active = true;
        InvokeOnPlay();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        escapeEvent = EventManager.Attach(() => true, () => {
            Vector3 distance = MovableObject.position - target.position;
            distance.y = 0;
            Vector3 direction = distance.normalized;
            walkBehaviour.Play(direction.x, direction.z, fitLookDirection);
            LookDirection = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        }, false);
    }

    public override void Stop()
    {
        if (Active)
        {
            InvokeOnStop();
            Active = false;
            EventManager.Detach(escapeEvent);
            walkBehaviour.speed.RemoveModifier(speedModifier);
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        }
    }
}
