using System;
using UnityEngine;

public delegate void JumpsChangedCallback(int jumps);

public class JumpCommand : ICommand
{
}

public class JumpBehaviour : BaseMovementBehaviour<JumpCommand>
{
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public int maxJumps;

    public event Action OnJump;
    public event JumpsChangedCallback OnJumpsChanged;
    public event Action OnLand;
    public event Action OnRecover;

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

    public int Jumps
    {
        get => jumps;
        private set
        {
            jumps = value;
            Animator.SetInteger(JumpsParameter, jumps);
        }
    }

    public override bool Playing => Anticipating || Active || Recovering;

    private bool active;
    private bool recovering;
    private bool anticipating;
    private int jumps;
    private string anticipateTimeout;
    private string recoverTimeout;
    private WalkBehaviour walkBehaviour;

    private static readonly int AnticipatingJumpParameter = Animator.StringToHash("anticipatingJump");
    private static readonly int JumpParameter = Animator.StringToHash("jump");
    private static readonly int RecoveringFromJumpParameter = Animator.StringToHash("recoveringFromJump");
    private static readonly int JumpsParameter = Animator.StringToHash("jumps");

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override bool CanPlay(JumpCommand command)
    {
        return base.CanPlay(command)
               && !AttackManager.Playing && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && (!Active || jumps < maxJumps);
    }

    protected override void DoPlay(JumpCommand command)
    {
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
        Jumps++;
        OnJump?.Invoke();
        OnJumpsChanged?.Invoke(Jumps);
        MovableObject.velocity.y = jumpSpeed;
        MovableObject.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
        MovableObject.OnLand += Land;
    }

    private void Land()
    {
        MovableObject.OnLand -= Land;

        Active = false;
        Jumps = 0;
        OnLand?.Invoke();
        OnJumpsChanged?.Invoke(Jumps);

        if (walkBehaviour && !walkBehaviour.Walk) //not moving
        {
            Recovering = true;
            walkBehaviour.Enabled = false;
            recoverTimeout = StartTimeout(() =>
            {
                walkBehaviour.Enabled = true;
                Stop();
            }, jumpRecoverTime);
        }
        else
        {
            Stop();
        }
    }

    protected override void DoStop()
    {
        if (Anticipating)
        {
            StopCoroutine(anticipateTimeout);
            walkBehaviour.Enabled = true;
            Anticipating = false;
        }

        if (Active)
        {
            MovableObject.velocity.y = 0;
            MovableObject.OnLand -= Land;
            Active = false;
            Jumps = 0;
            OnJumpsChanged?.Invoke(Jumps);
        }

        if (Recovering)
        {
            StopCoroutine(recoverTimeout);
            walkBehaviour.Enabled = true;
            Recovering = false;
            OnRecover?.Invoke();
        }
    }
}