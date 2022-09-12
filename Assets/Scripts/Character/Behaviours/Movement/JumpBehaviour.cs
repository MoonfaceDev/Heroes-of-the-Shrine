using System;
using System.Collections;
using UnityEngine;

public class JumpBehaviour : CharacterBehaviour
{
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public int maxJumps;

    public delegate void OnJumpsChanged(int jumps);

    public event Action onAnticipate;
    public event Action onJump;
    public event OnJumpsChanged onJumpsChanged;
    public event Action onLand;
    public event Action onRecover;
    public event Action onStop;

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
    public bool jump
    {
        get => anticipating || active || recovering;
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

    private bool _active;
    private bool _recovering;
    private bool _anticipating;
    private int _jumps;
    private Coroutine anticipateCoroutine;
    private EventListener jumpEvent;
    private Coroutine recoverCoroutine;
    private WalkBehaviour walkBehaviour;
    private SlideBehaviour slideBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private AttackManager attackManager;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
        attackManager = GetComponent<AttackManager>();
    }

    public bool CanJump()
    {
        return !anticipating
            && !recovering 
            && jumps < maxJumps
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager && attackManager.attacking);
    }

    public void Jump()
    {
        if (!CanJump())
        {
            return;
        }
        if (walkBehaviour && walkBehaviour.walk == false && movableObject.position.y == 0) //not moving and grounded
        {
            anticipating = true;
            onAnticipate?.Invoke();
            anticipateCoroutine = StartCoroutine(Anticipate());
        }
        else //moving or mid-air
        {
            StartJump();
        }
    }

    private IEnumerator Anticipate()
    {
        yield return new WaitForSeconds(jumpAnticipateTime);
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
        recovering = true;
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
            Recover();
        }
    }

    private IEnumerator RecoverAfterTime()
    {
        yield return new WaitForSeconds(jumpRecoverTime);
        Recover();
    }

    private void Recover()
    {
        recovering = false;
        onRecover?.Invoke();
    }

    public void Stop(bool waitForLand = true)
    {
        if (anticipating)
        {
            StopCoroutine(anticipateCoroutine);
            anticipating = false;
        }
        if (active)
        {
            movableObject.velocity.y = 0;
            if (!waitForLand)
            {
                eventManager.Detach(jumpEvent);
                active = false;
                jumps = 0;
                onJumpsChanged?.Invoke(jumps);
            }
        }
        onStop?.Invoke();
        if (recovering)
        {
            StopCoroutine(recoverCoroutine);
            Recover();
        }
    }
}
