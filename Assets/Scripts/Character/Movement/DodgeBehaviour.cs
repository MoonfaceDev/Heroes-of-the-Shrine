using System;
using UnityEngine;

public class DodgeCommand : ICommand
{
    public readonly int direction;

    public DodgeCommand(int direction)
    {
        this.direction = direction;
    }
}

public class DodgeBehaviour : BaseMovementBehaviour<DodgeCommand>
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
    private string anticipateTimeout;
    private string recoverTimeout;

    private static readonly int RecoveringFromDodgeParameter = Animator.StringToHash("recoveringFromDodge");
    private static readonly int AnticipatingDodgeParameter = Animator.StringToHash("anticipatingDodge");

    public override bool CanPlay(DodgeCommand command)
    {
        var attackManager = GetComponent<AttackManager>();
        return base.CanPlay(command)
               && !IsPlaying<JumpBehaviour>() && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && command.direction != 0
               && !(attackManager && !attackManager.IsInterruptible());
    }

    protected override void DoPlay(DodgeCommand command)
    {
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour), typeof(BaseAttack));

        Anticipating = true;

        anticipateTimeout = StartTimeout(() =>
        {
            Anticipating = false;
            MovableEntity.UpdatePosition(MovableEntity.position + command.direction * dodgeDistance * Vector3.forward);
            OnDodge?.Invoke();
            Recovering = true;
            recoverTimeout = StartTimeout(Stop, recoveryTime);
        }, anticipateTime);
    }

    protected override void DoStop()
    {
        EnableBehaviours(typeof(WalkBehaviour));

        if (Anticipating)
        {
            Cancel(anticipateTimeout);
            Anticipating = false;
        }

        if (Recovering)
        {
            Cancel(recoverTimeout);
            Recovering = false;
            OnRecover?.Invoke();
        }
    }
}