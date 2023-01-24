using UnityEngine;
using UnityEngine.Events;


public class JumpBehaviour : BaseMovementBehaviour<JumpBehaviour.Command>
{
    public class Command
    {
    }

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

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && !(AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering)
               && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && !Playing;
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(BaseAttack));
        
        if (!IsPlaying<WalkBehaviour>() && MovableEntity.WorldPosition.y == 0) //not moving and grounded
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
        
        MovableEntity.velocity.y = jumpSpeed;
        MovableEntity.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
        
        MovableEntity.OnLand += Land;
    }

    private void Land()
    {
        MovableEntity.OnLand -= Land;

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
            MovableEntity.velocity.y = 0;
            MovableEntity.OnLand -= Land;
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