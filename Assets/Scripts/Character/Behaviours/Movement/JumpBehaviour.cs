using UnityEngine;
using UnityEngine.Events;

public class JumpCommand : ICommand
{
}

public class JumpBehaviour : BaseMovementBehaviour<JumpCommand>
{
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;

    public UnityEvent onStartActive;
    public UnityEvent onFinishActive;

    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(AnticipatingJumpParameter, anticipating);
        }
    }

    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(JumpParameter, active);
            (value ? onStartActive : onFinishActive).Invoke();
        }
    }

    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(RecoveringFromJumpParameter, recovering);
        }
    }

    public override bool Playing => Anticipating || Active || Recovering;

    private bool active;
    private bool recovering;
    private bool anticipating;
    private string anticipateTimeout;
    private string recoverTimeout;
    private WalkBehaviour walkBehaviour;

    private static readonly int AnticipatingJumpParameter = Animator.StringToHash("anticipatingJump");
    private static readonly int JumpParameter = Animator.StringToHash("jump");
    private static readonly int RecoveringFromJumpParameter = Animator.StringToHash("recoveringFromJump");

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override bool CanPlay(JumpCommand command)
    {
        return base.CanPlay(command)
               && !(AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering)
               && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && !Playing;
    }

    protected override void DoPlay(JumpCommand command)
    {
        StopBehaviours(typeof(BaseAttack));
        
        if (!IsPlaying<WalkBehaviour>() && MovableObject.WorldPosition.y == 0) //not moving and grounded
        {
            Anticipating = true;
            
            walkBehaviour.Enabled = false;
            anticipateTimeout = StartTimeout(() =>
            {
                walkBehaviour.Enabled = true;
                Anticipating = false;
                StartJump();
            }, jumpAnticipateTime);
        }
        else //moving or mid-air
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        Active = true;
        
        MovableObject.velocity.y = jumpSpeed;
        MovableObject.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
        
        MovableObject.OnLand += Land;
    }

    private void Land()
    {
        MovableObject.OnLand -= Land;

        Active = false;
        Recovering = true;

        walkBehaviour.Enabled = false;
        recoverTimeout = StartTimeout(Stop, jumpRecoverTime);
    }

    protected override void DoStop()
    {
        if (Anticipating)
        {
            Cancel(anticipateTimeout);
            walkBehaviour.Enabled = true;
            Anticipating = false;
        }

        if (Active)
        {
            MovableObject.velocity.y = 0;
            MovableObject.OnLand -= Land;
            Active = false;
        }

        if (Recovering)
        {
            Cancel(recoverTimeout);
            walkBehaviour.Enabled = true;
            Recovering = false;
        }
    }
}