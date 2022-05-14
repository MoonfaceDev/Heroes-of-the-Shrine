using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class SimpleCharacterMovement : MonoBehaviour, ICharacterMovement
{
    public static float gravityAcceleration = 20;

    //general
    [Header("General")]
    private MovableObject movableObject;
    public EventManager eventManager;
    public Animator animator;

    // look direction
    private int lookDirection = 1;
    public int LookDirection
    {
        get
        {
            return lookDirection;
        }
        set
        {
            lookDirection = value;
            transform.rotation = Quaternion.Euler(0, 90 * lookDirection - 90, 0);
        }
    }

    //walk
    [Header("Walk")]
    public float speed;

    //run
    [Header("Run")]
    public float timeToRun;
    public float runSpeedMultiplier;
    private Coroutine startRunningCoroutine;
    public ParticleSystem runParticles;
    private ParticleSystem.MainModule runParticlesMain;

    //jump
    [Header("Jump")]
    public float jumpSpeed;
    public float jumpAnticipateTime;
    public float jumpRecoverTime;
    public int maxJumps;

    //slide
    [Header("Slide")]
    public float slideStartSpeed;
    public float slideStopAcceleration;

    //knockback
    [Header("Knockback")]
    public float knockbackRecoverTime;
    public float secondBounceHeight;

    //stun
    [HideInInspector] public bool stun;

    private bool _walk; //walking or running
    private bool _run;
    private bool _jump;
    private bool _recoveringFromJump;
    private bool _anticipatingJump;
    private bool _slide;
    private bool _knockback;
    private bool _recoveringFromKnockback;

    [HideInInspector] public bool grounded = true;
    public bool walk
    {
        get { return _walk; }
        set { _walk = value; animator.SetBool("walk", _walk); }
    }
    public bool run
    {
        get { return _run; }
        set { _run = value; animator.SetBool("run", _run); }
    }
    public bool jump
    {
        get { return _jump; }
        set { _jump = value; animator.SetBool("jump", _jump); }
    }
    public bool recoveringFromJump
    {
        get { return _recoveringFromJump; }
        set { _recoveringFromJump = value; animator.SetBool("recoveringFromJump", _recoveringFromJump); }
    }
    public bool anticipatingJump
    {
        get { return _anticipatingJump; }
        set { _anticipatingJump = value; animator.SetBool("anticipatingJump", _anticipatingJump); }
    }
    [HideInInspector] public int jumps;
    public bool slide
    {
        get { return _slide; }
        set { _slide = value; animator.SetBool("slide", _slide); }
    }
    public bool recoveringFromKnockback
    {
        get { return _recoveringFromKnockback; }
        set { _recoveringFromKnockback = value; animator.SetBool("recoveringFromKnockback", _recoveringFromKnockback); }
    }
    public bool knockback
    {
        get { return _knockback; }
        set { _knockback = value; animator.SetBool("knockback", _knockback); }
    }


    //events
    class LookDirectionEvent : IEventListener
    {
        private readonly SimpleCharacterMovement characterMovement;

        public LookDirectionEvent(SimpleCharacterMovement characterMovement)
        {
            this.characterMovement = characterMovement;
        }

        public void Callback()
        {
            if (characterMovement.knockback)
            {
                characterMovement.LookDirection = -Mathf.RoundToInt(Mathf.Sign(characterMovement.movableObject.velocity.x));
            }
            else
            {
                characterMovement.LookDirection = Mathf.RoundToInt(Mathf.Sign(characterMovement.movableObject.velocity.x));
            }
        }

        public bool Validate()
        {
            return Mathf.Abs(characterMovement.movableObject.velocity.x) > Mathf.Epsilon;
        }
    }

    // Use this for initialization
    void Start()
    {
        movableObject = GetComponent<MovableObject>();
        runParticlesMain = runParticles.main;
        //Update look direction
        LookDirectionEvent lookDirectionEvent = new(this);
        eventManager.AttachEvent(lookDirectionEvent);
    }

    #region Knockback
    public void Knockback(float direction, float distance, float height)
    {
        EndJump();
        float airTime = 2 * Mathf.Sqrt(2 * height / gravityAcceleration);
        movableObject.velocity.x = direction * distance / airTime;
        movableObject.velocity.y = Mathf.Sqrt(2 * gravityAcceleration * height);
        movableObject.acceleration.y = -gravityAcceleration;
        knockback = true;
        grounded = false;
        eventManager.Callback(
            () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
            () => 
            {
                movableObject.velocity.y = Mathf.Sqrt(2 * gravityAcceleration * height);
                movableObject.acceleration.y = -gravityAcceleration;
                movableObject.position.y = 0;
                eventManager.Callback(
                    () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
                    () =>
                    {
                        movableObject.acceleration.y = 0;
                        movableObject.velocity.y = 0;
                        movableObject.velocity.x = 0;
                        movableObject.position.y = 0;
                        knockback = false;
                        grounded = true;
                        recoveringFromKnockback = true;
                        StartCoroutine(RecoverFromKnockback());
                    }
                );
            }
        );
    }

    public IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(knockbackRecoverTime);
        EndKnockback();
    }

    public void EndKnockback()
    {
        recoveringFromKnockback = false;
        knockback = false;
    }
    #endregion

    #region Stun
    public void Stun(float time)
    {
        StartCoroutine(RemoveStunAfter(time));
    }

    private IEnumerator RemoveStunAfter(float time)
    {
        yield return new WaitForSeconds(time);
    }
    #endregion

    #region Run
    public void StartRunning()
    {
        run = true;
        runParticles.Play();
    }

    public void StopRunning()
    {
        run = false;
        runParticles.Stop();
    }
    #endregion

    #region Walk
    public bool CanWalk()
    {
        return !anticipatingJump && !recoveringFromJump && !slide && !knockback && !stun;
    }

    public void Walk(float xAxis, float zAxis)
    {
        if (!CanWalk())
        {
            return;
        }
        // run callbacks
        if (new Vector2(xAxis, zAxis) == Vector2.zero)
        {
            walk = false;
            run = false;
            if (startRunningCoroutine != null)
            {
                StopCoroutine(startRunningCoroutine);
            }
        }
        else if (!walk) //first walking frame
        {
            walk = true;
            startRunningCoroutine = StartCoroutine(StartRunningAfter(timeToRun));
            eventManager.Callback(() => !walk, () => StopCoroutine(startRunningCoroutine));
        }
        // move speed
        if (run)
        {
            movableObject.velocity.x = xAxis * runSpeedMultiplier * speed;
        }
        else
        {
            movableObject.velocity.x = xAxis * speed;
        }
        movableObject.velocity.z = zAxis * speed;
    }

    private IEnumerator StartRunningAfter(float time)
    {
        yield return new WaitForSeconds(time);
        StartRunning();
    }
    #endregion

    #region Jump
    public bool CanJump()
    {
        return !anticipatingJump && !recoveringFromJump && jumps < maxJumps && !slide && !knockback && !stun;
    }

    public void Jump()
    {
        if(!CanJump())
        {
            return;
        }
        jump = true;
        grounded = false;
        jumps++;
        anticipatingJump = true;
        if (walk == false && grounded) //not moving and grounded
        {
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
        StartJump();
    }

    void StartJump()
    {
        anticipatingJump = false;
        movableObject.velocity.y = jumpSpeed;
        movableObject.acceleration.y = -gravityAcceleration;
        runParticlesMain.gravityModifier = 1f;
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
        if (walk) //moving
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
        EndJump();
    }

    public void EndJump()
    {
        recoveringFromJump = false;
        jump = false;
        grounded = true;
        jumps = 0;
        runParticlesMain.gravityModifier = 0;
    }

    #endregion

    #region Slide
    public bool CanSlide()
    {
        return movableObject.velocity.x != 0 && grounded && !slide && !knockback && !stun;
    }

    public void Slide()
    {
        if(!CanSlide())
        {
            return;
        }
        walk = false;
        float slideDirection = lookDirection;
        if (!run)
        {
            movableObject.velocity.x = slideDirection * slideStartSpeed;
        }
        else
        {
            movableObject.velocity.x = slideDirection * slideStartSpeed * runSpeedMultiplier;
        }
        movableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        movableObject.velocity.z = 0;
        slide = true;
        eventManager.Callback(
            () => Mathf.Sign(movableObject.velocity.x) == Mathf.Sign(movableObject.acceleration.x),
            EndSlide
        );
    }

    public void EndSlide()
    {
        slide = false;
        LookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
    }
    #endregion
}