using System;
using UnityEngine;

public class DodgeBehaviour : BaseMovementBehaviour<DodgeBehaviour.Command>
{
    public class Command
    {
        public readonly int direction;

        public Command(int direction)
        {
            this.direction = direction;
        }
    }
    
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

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && !IsPlaying<JumpBehaviour>() && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && command.direction != 0
               && !(AttackManager && !AttackManager.CanPlayMove(true));
    }

    protected override void DoPlay(Command command)
    {
        BlockBehaviours(typeof(WalkBehaviour));
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
        UnblockBehaviours(typeof(WalkBehaviour));

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