using System.Collections;
using UnityEngine;

public class JumpBehaviour : CharacterBehaviour
{
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public int maxJumps;

    public delegate void OnAnticipate();
    public delegate void OnJump();
    public delegate void OnLand();
    public delegate void OnRecover();

    public event OnAnticipate onAnticipate;
    public event OnJump onJump;
    public event OnLand onLand;
    public event OnRecover onRecover;
    public bool jump
    {
        get { return _jump; }
        set { 
            _jump = value;
            animator.SetBool("jump", _jump);
            if (value)
            {
                onJump?.Invoke();
            }
            else
            {
                onLand?.Invoke();
            }
        }
    }
    public bool recoveringFromJump
    {
        get { return _recoveringFromJump; }
        set { 
            _recoveringFromJump = value;
            animator.SetBool("recoveringFromJump", _recoveringFromJump);
            if (!value)
            {
                onRecover?.Invoke();
            }
        }
    }
    public bool anticipatingJump
    {
        get { return _anticipatingJump; }
        set { 
            _anticipatingJump = value;
            animator.SetBool("anticipatingJump", _anticipatingJump);
            if (value)
            {
                onAnticipate?.Invoke();
            }
        }
    }
    [HideInInspector] public int jumps;

    private bool _jump;
    private bool _recoveringFromJump;
    private bool _anticipatingJump;
    private WalkBehaviour walkBehaviour;
    private SlideBehaviour slideBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanJump()
    {
        return !anticipatingJump
            && !recoveringFromJump 
            && jumps < maxJumps
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && (knockbackBehaviour.knockback || knockbackBehaviour.recoveringFromKnockback))
            && !(stunBehaviour && stunBehaviour.stun);
    }

    public void Jump()
    {
        if (!CanJump())
        {
            return;
        }
        if (walkBehaviour && walkBehaviour.walk == false && movableObject.position.y == 0) //not moving and grounded
        {
            anticipatingJump = true;
            StartCoroutine(AnticipateJump());
        }
        else //moving or mid-air
        {
            StartJump();
        }
    }

    IEnumerator AnticipateJump()
    {
        yield return new WaitForSeconds(jumpAnticipateTime);
        anticipatingJump = false;
        StartJump();
    }

    void StartJump()
    {
        jump = true;
        jumps++;
        movableObject.velocity.y = jumpSpeed;
        movableObject.acceleration.y = -gravityAcceleration;
        eventManager.Callback(
            () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
            Land
        );
    }

    public void Land()
    {
        movableObject.position.y = 0;
        movableObject.velocity.y = 0;
        movableObject.acceleration.y = 0;
        jump = false;
        if (walkBehaviour && walkBehaviour.walk) //moving
        {
            EndJump();
        }
        else //not moving
        {
            recoveringFromJump = true;
            StartCoroutine(RecoverFromJump());
        }
    }

    IEnumerator RecoverFromJump()
    {
        yield return new WaitForSeconds(jumpRecoverTime);
        recoveringFromJump = false;
        EndJump();
    }

    public void EndJump()
    {
        jumps = 0;
    }
}
