using ExtEvents;
using UnityEngine;

public class JumpBehaviour : BaseMovementBehaviour<JumpBehaviour.Command>
{
    public class Command
    {
    }

    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public float climbAcceleration;
    public float peakDuration;
    public float peakAcceleration;

    [SerializeField] public ExtEvent onStartActive;
    [SerializeField] public ExtEvent onFinishActive;

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
    private string stopClimbListener;
    private string stopPeakListener;
    
    private WalkBehaviour walkBehaviour;

    private static readonly int AnticipatingJumpParameter = Animator.StringToHash("anticipatingJump");
    private static readonly int JumpParameter = Animator.StringToHash("jump");
    private static readonly int RecoveringFromJumpParameter = Animator.StringToHash("recoveringFromJump");

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
    }

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && !(AttackManager && !AttackManager.CanPlayMove())
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
        MovableEntity.acceleration.y = -climbAcceleration;

        stopClimbListener = InvokeWhen(() => MovableEntity.velocity.y < 0, () =>
        {
            MovableEntity.acceleration.y = -peakAcceleration;
            var peakStartTime = Time.time;
            stopPeakListener = InvokeWhen(
                () => Time.time - peakStartTime > peakDuration,
                () => MovableEntity.acceleration.y = -Character.physicalAttributes.gravityAcceleration
            );
        });

        MovableEntity.OnLand += Land;
    }

    public void Freeze()
    {
        Cancel(stopClimbListener);
        Cancel(stopPeakListener);
        MovableEntity.velocity.y = 0;
        MovableEntity.acceleration.y = 0;
    }

    public void Unfreeze()
    {
        MovableEntity.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
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
            Cancel(stopClimbListener);
            Cancel(stopPeakListener);
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