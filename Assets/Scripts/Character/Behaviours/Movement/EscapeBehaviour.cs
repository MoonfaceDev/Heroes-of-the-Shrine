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
    private IModifier speedModifier;
    private EventListener escapeEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        movableObject.onStuck += () => {
            if (active)
            {
                Stop();
            }
        };
        walkBehaviour.onStop += () =>
        {
            if (active)
            {
                Stop();
            }
        };
    }

    public void Escape(MovableObject target, float speedMultiplier, bool fitLookDirection = true)
    {
        active = true;
        onStart?.Invoke();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        escapeEvent = eventManager.Attach(() => true, () => {
            Vector3 distance = movableObject.position - target.position;
            distance.y = 0;
            Vector3 direction = distance.normalized;
            walkBehaviour.Walk(direction.x, direction.z, fitLookDirection);
            lookDirection = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        }, false);
    }

    public override void Stop()
    {
        if (active)
        {
            onStop?.Invoke();
            active = false;
            eventManager.Detach(escapeEvent);
            walkBehaviour.Stop(true);
            walkBehaviour.speed.RemoveModifier(speedModifier);
        }
    }
}
