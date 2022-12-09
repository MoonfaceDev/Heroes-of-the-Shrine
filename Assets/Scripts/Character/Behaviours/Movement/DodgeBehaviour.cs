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
            Animator.SetBool(RecoveringFromDodgeParameter, recovering);
        }
    }
    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(AnticipatingDodgeParameter, anticipating);
        }
    }

    public override bool Playing => Anticipating || Recovering;

    private bool anticipating;
    private bool recovering;
    private EventListener anticipateEvent;
    private EventListener recoverEvent;
    
    private static readonly int RecoveringFromDodgeParameter = Animator.StringToHash("recoveringFromDodge");
    private static readonly int AnticipatingDodgeParameter = Animator.StringToHash("anticipatingDodge");

    public override bool CanPlay()
    {
        var attackManager = GetComponent<AttackManager>();
        return base.CanPlay()
            && AllStopped(typeof(JumpBehaviour), typeof(SlideBehaviour)) 
            && !(attackManager && !attackManager.IsInterruptible());
    }

    public void Play(int direction)
    {
        if (!(CanPlay() && direction != 0))
        {
            return;
        }
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour), typeof(AttackManager));
        Anticipating = true;
        InvokeOnPlay();
        MovableObject.acceleration = Vector3.zero;
        MovableObject.velocity = Vector3.zero;
        anticipateEvent = EventManager.StartTimeout(() =>
        {
            Anticipating = false;
            MovableObject.UpdatePosition(MovableObject.position + direction * dodgeDistance * Vector3.forward);
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
