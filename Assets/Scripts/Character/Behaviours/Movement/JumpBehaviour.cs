using System;
using System.Collections;
using UnityEngine;

public delegate void JumpsChangedCallback(int jumps);

public class JumpBehaviour : BaseMovementBehaviour
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
        private set { 
            active = value;
            Animator.SetBool(JumpParameter, active);
        }
    }
    public bool Recovering
    {
        get => recovering;
        private set { 
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
    private Coroutine anticipateCoroutine;
    private Coroutine recoverCoroutine;
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

    public override bool CanPlay()
    {
        return base.CanPlay() 
            && AllStopped(typeof(AttackManager), typeof(SlideBehaviour), typeof(DodgeBehaviour))
            && (!Active || jumps < maxJumps);
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        onPlay.Invoke();
        if (!IsPlaying(typeof(WalkBehaviour)) && MovableObject.WorldPosition.y == 0) //not moving and grounded
        {
            anticipateCoroutine = StartCoroutine(Anticipate());
        }
        else //moving or mid-air
        {
            StartJump();
        }
    }

    private IEnumerator Anticipate()
    {
        Anticipating = true;
        walkBehaviour.Enabled = false;
        yield return new WaitForSeconds(jumpAnticipateTime);
        walkBehaviour.Enabled = true;
        Anticipating = false;
        StartJump();
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
            recoverCoroutine = StartCoroutine(RecoverAfterTime());
        }
        else
        {
            onStop.Invoke();
        }
    }

    private IEnumerator RecoverAfterTime()
    {
        Recovering = true;
        walkBehaviour.Enabled = false;
        yield return new WaitForSeconds(jumpRecoverTime);
        walkBehaviour.Enabled = true;
        Recovering = false;
        OnRecover?.Invoke();
    }

    public override void Stop()
    {
        if (Playing)
        {
            onStop.Invoke();
        }
        if (Anticipating)
        {
            StopCoroutine(anticipateCoroutine);
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
            StopCoroutine(recoverCoroutine);
            walkBehaviour.Enabled = true;
            Recovering = false;
            OnRecover?.Invoke();
        }
    }
}
