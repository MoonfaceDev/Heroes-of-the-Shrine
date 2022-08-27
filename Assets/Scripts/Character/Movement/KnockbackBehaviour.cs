using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBehaviour : CharacterBehaviour
{
    public float knockbackRecoverTime;
    public float secondBounceHeight;

    public delegate void OnStart();
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
            if (value)
            {
                onStart();
            }
            else
            {
                onStop();
            }
        }
    }

    private bool _knockback;
    private bool _recoveringFromKnockback;
    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
    }

    public void Knockback(float direction, float distance, float height)
    {
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
        float airTime = 2 * Mathf.Sqrt(2 * height / gravityAcceleration);
        movableObject.velocity.x = direction * distance / airTime;
        movableObject.velocity.y = Mathf.Sqrt(2 * gravityAcceleration * height);
        movableObject.acceleration.y = -gravityAcceleration;
        knockback = true;
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
