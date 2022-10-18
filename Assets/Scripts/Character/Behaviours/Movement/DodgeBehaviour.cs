using System;
using UnityEngine;

public class DodgeBehaviour : BaseMovementBehaviour
{
    public float dodgeDistance;
    public float anticipateTime;
    public float recoveryTime;

    public event Action OnDodge;
    public event Action OnRecover;

    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool("recoveringFromDodge", recovering);
        }
    }
    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool("anticipatingDodge", anticipating);
        }
    }

    public override bool Playing => Anticipating || Recovering;

    private bool anticipating;
    private bool recovering;
    private EventListener anticipateEvent;
    private EventListener recoverEvent;

    public override bool CanPlay()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return base.CanPlay()
            && AllStopped(typeof(JumpBehaviour), typeof(SlideBehaviour)) 
            && !(attackManager && !attackManager.IsInterruptable())
            && MovableObject.velocity.z != 0 && MovableObject.velocity.x == 0;
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour));
        Anticipating = true;
        InvokeOnPlay();
        float dodgeDirection = Mathf.Sign(MovableObject.velocity.z);
        MovableObject.acceleration = Vector3.zero;
        MovableObject.velocity = Vector3.zero;
        anticipateEvent = EventManager.StartTimeout(() =>
        {
            Anticipating = false;
            MovableObject.UpdatePosition(MovableObject.position + dodgeDirection * dodgeDistance * Vector3.forward);
            OnDodge?.Invoke();
            Recovering = true;
            recoverEvent = EventManager.StartTimeout(Stop, recoveryTime);
        }, anticipateTime);
    }

    public override void Stop()
    {
        if (Playing)
        {
            InvokeOnStop();
            EnableBehaviours(typeof(WalkBehaviour));
        }
        if (Anticipating)
        {
            EventManager.Detach(anticipateEvent);
            Anticipating = false;
        }
        if (Recovering)
        {
            EventManager.Detach(recoverEvent);
            Recovering = false;
            OnRecover?.Invoke();
        }
    }
}
