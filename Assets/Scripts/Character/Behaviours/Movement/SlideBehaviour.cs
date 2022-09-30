using System;
using UnityEngine;

public class SlideBehaviour : CharacterBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public event Action onStart;
    public event Action onFinish;
    public bool slide
    {
        get => _slide;
        private set { 
            _slide = value;
            animator.SetBool("slide", _slide);
        }
    }

    private bool _slide;
    private WalkBehaviour walkBehaviour;
    private EventListener stopEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public bool CanSlide()
    {
        DodgeBehaviour dodgeBehaviour = GetComponent<DodgeBehaviour>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();
        return movableObject.velocity.x != 0
            && movableObject.position.y == 0
            && !slide
            && !(dodgeBehaviour && dodgeBehaviour.dodge)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
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
        slide = true;
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
        slide = false;
        lookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
        onFinish?.Invoke();
    }

    public void Stop()
    {
        eventManager.Detach(stopEvent);
        LeaveSlide();
    }
}
