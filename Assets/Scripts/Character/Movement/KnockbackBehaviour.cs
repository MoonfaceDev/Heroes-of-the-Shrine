using System.Collections;
using UnityEngine;

public class KnockbackBehaviour : CharacterBehaviour
{
    public static float SECOND_BOUNCE_POWER_MULTIPLIER = 0.2f;

    public bool resistant;
    public float knockbackRecoverTime;

    public delegate void OnStart(float power, float angleDegrees);
    public delegate void OnStop();
    public delegate void OnRecover();

    public OnStart onStart;
    public OnStop onStop;
    public OnRecover onRecover;
    public bool recoveringFromKnockback
    {
        get { return _recoveringFromKnockback; }
        set { 
            _recoveringFromKnockback = value;
            animator.SetBool("recoveringFromKnockback", _recoveringFromKnockback);
            if (!value)
            {
                onRecover();
            }
        }
    }
    public bool knockback
    {
        get { return _knockback; }
        set { 
            _knockback = value;
            animator.SetBool("knockback", _knockback);
            if (!value)
            {
                onStop();
            }
        }
    }

    private bool _knockback;
    private bool _recoveringFromKnockback;
    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private StunBehaviour stunBehaviour;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanReceive()
    {
        return !knockback
            && !recoveringFromKnockback
            && !resistant
            && !(stunBehaviour && stunBehaviour.stun);
    }

    public void Knockback(float power, float angleDegrees)
    {
        if (!CanReceive())
        {
            return;
        }
        onStart(power, angleDegrees);
        if (walkBehaviour)
        {
            walkBehaviour.walk = false;
        }
        if (jumpBehaviour)
        {
            jumpBehaviour.anticipatingJump = false;
            jumpBehaviour.recoveringFromJump = false;
            jumpBehaviour.EndJump();
        }
        movableObject.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angleDegrees) * power;
        movableObject.velocity.y = Mathf.Sin(Mathf.Deg2Rad * angleDegrees) * power;
        movableObject.acceleration.y = -gravityAcceleration;
        knockback = true;
        eventManager.Callback(
            () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
            () =>
            {
                movableObject.velocity.x *= SECOND_BOUNCE_POWER_MULTIPLIER;
                movableObject.velocity.y *= -SECOND_BOUNCE_POWER_MULTIPLIER;
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
}
