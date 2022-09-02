using UnityEngine;

public class SlideBehaviour : CharacterBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public delegate void OnStart();
    public delegate void OnStop();

    public event OnStart onStart;
    public event OnStop onStop;
    public bool active
    {
        get => _active;
        private set { 
            _active = value;
            animator.SetBool("slide", _active);
        }
    }

    private bool _active;
    private WalkBehaviour walkBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private AttackManager attackManager;
    private EventListener stopEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
        attackManager = GetComponent<AttackManager>();
    }

    public bool CanSlide()
    {
        return movableObject.velocity.x != 0 
            && movableObject.position.y == 0
            && !active
            && !(knockbackBehaviour && (knockbackBehaviour.active || knockbackBehaviour.recovering))
            && !(stunBehaviour && stunBehaviour.active)
            && !(attackManager && attackManager.attacking);
    }

    public void Slide()
    {
        if (!CanSlide())
        {
            return;
        }
        if (walkBehaviour)
        {
            walkBehaviour.Stop();
        }
        active = true;
        onStart?.Invoke();
        float slideDirection = lookDirection;
        movableObject.velocity.x = slideDirection * slideSpeedMultiplier * Mathf.Abs(movableObject.velocity.x);
        movableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        movableObject.velocity.z = 0;
        stopEvent = eventManager.Attach(
            () => Mathf.Sign(movableObject.velocity.x) == Mathf.Sign(movableObject.acceleration.x),
            LeaveSlide
        );
    }

    private void LeaveSlide()
    {
        active = false;
        lookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
        onStop?.Invoke();
    }

    public void Stop()
    {
        eventManager.Detach(stopEvent);
        LeaveSlide();
    }
}
