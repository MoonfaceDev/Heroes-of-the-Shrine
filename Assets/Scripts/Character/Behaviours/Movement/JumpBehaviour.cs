using System;
using System.Collections;
using UnityEngine;

public class JumpBehaviour : SoloMovementBehaviour
{
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public int maxJumps;

    public delegate void OnJumpsChanged(int jumps);

    public event Action onJump;
    public event OnJumpsChanged onJumpsChanged;
    public event Action onLand;
    public event Action onRecover;

    public bool active
    {
        get => _active;
        private set { 
            _active = value;
            animator.SetBool("jump", _active);
        }
    }
    public bool recovering
    {
        get => _recovering;
        private set { 
            _recovering = value;
            animator.SetBool("recoveringFromJump", _recovering);
        }
    }
    public bool anticipating
    {
        get => _anticipating;
        private set { 
            _anticipating = value;
            animator.SetBool("anticipatingJump", _anticipating);
        }
    }
    public int jumps
    {
        get => _jumps;
        private set
        {
            _jumps = value;
            animator.SetInteger("jumps", _jumps);
        }
    }

    public override bool Playing => anticipating || active || recovering;

    private bool _active;
    private bool _recovering;
    private bool _anticipating;
    private int _jumps;
    private Coroutine anticipateCoroutine;
    private EventListener jumpEvent;
    private Coroutine recoverCoroutine;
    private WalkBehaviour walkBehaviour;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public override bool CanPlay()
    {
        return base.CanPlay() || (active && jumps < maxJumps);
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        InvokeOnPlay();
        if (!IsPlaying(typeof(WalkBehaviour)) && movableObject.position.y == 0) //not moving and grounded
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
        anticipating = true;
        walkBehaviour.Enabled = false;
        yield return new WaitForSeconds(jumpAnticipateTime);
        walkBehaviour.Enabled = true;
        anticipating = false;
        StartJump();
    }

    private void StartJump()
    {
        active = true;
        jumps++;
        onJump?.Invoke();
        onJumpsChanged?.Invoke(jumps);
        movableObject.velocity.y = jumpSpeed;
        movableObject.acceleration.y = -gravityAcceleration;
        jumpEvent = eventManager.Attach(
            () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
            Land
        );
    }

    private void Land()
    {
        active = false;
        jumps = 0;
        onLand?.Invoke();
        onJumpsChanged?.Invoke(jumps);

        movableObject.position.y = 0;
        movableObject.velocity.y = 0;
        movableObject.acceleration.y = 0;

        if (walkBehaviour && !walkBehaviour.walk) //not moving
        {
            recoverCoroutine = StartCoroutine(RecoverAfterTime());
        }
        else
        {
            InvokeOnStop();
        }
    }

    private IEnumerator RecoverAfterTime()
    {
        recovering = true;
        walkBehaviour.Enabled = false;
        yield return new WaitForSeconds(jumpRecoverTime);
        walkBehaviour.Enabled = true;
        recovering = false;
        onRecover?.Invoke();
    }

    public override void Stop()
    {
        if (Playing)
        {
            InvokeOnStop();
        }
        if (anticipating)
        {
            StopCoroutine(anticipateCoroutine);
            walkBehaviour.Enabled = true;
            anticipating = false;
        }
        if (active)
        {
            movableObject.velocity.y = 0;
            eventManager.Detach(jumpEvent);
            active = false;
            jumps = 0;
            onJumpsChanged?.Invoke(jumps);
        }
        if (recovering)
        {
            StopCoroutine(recoverCoroutine);
            walkBehaviour.Enabled = true;
            recovering = false;
            onRecover?.Invoke();
        }
    }
}
