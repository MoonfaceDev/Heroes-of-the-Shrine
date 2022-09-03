using System.Collections;
using UnityEngine;

public class KnockbackBehaviour : CharacterBehaviour
{
    public static float SECOND_BOUNCE_POWER_MULTIPLIER = 0.2f;

    public bool resistant;
    public float knockbackRecoverTime;

    public delegate void OnStart();
    public delegate void OnBounce(int count, float power, float angleDegrees);
    public delegate void OnFinish();
    public delegate void OnRecover();

    public event OnStart onStart;
    public event OnBounce onBounce;
    public event OnFinish onFinish;
    public event OnRecover onRecover;
    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
            animator.SetBool("knockback", _active);
        }
    }
    public bool recovering
    {
        get => _recovering;
        private set
        {
            _recovering = value;
            animator.SetBool("recoveringFromKnockback", _recovering);
        }
    }
    public bool knockback
    {
        get => active || recovering;
    }
    public int bounce
    {
        get => _bounce;
        private set
        {
            _bounce = value;
            animator.SetInteger("bounce", _bounce);
        }
    }

    private bool _active;
    private bool _recovering;
    private int _bounce;
    private EventListener bounceEvent;
    private Coroutine recoverCoroutine;
    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private StunBehaviour stunBehaviour;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanReceive()
    {
        return !active
            && !recovering
            && !resistant
            && !(stunBehaviour && stunBehaviour.stun);
    }

    public void Knockback(float power, float angleDegrees)
    {
        if (!CanReceive())
        {
            return;
        }

        if (walkBehaviour)
        {
            walkBehaviour.Stop();
        }

        if (jumpBehaviour)
        {
            jumpBehaviour.Stop(waitForLand: false);
        }

        active = true;
        onStart?.Invoke();
        bounce = 1;
        SetMovement(power, angleDegrees);
        onBounce?.Invoke(bounce, power, angleDegrees);

        bounceEvent = eventManager.Attach(
            () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
            () =>
            { 
                bounce = 2;
                power *= SECOND_BOUNCE_POWER_MULTIPLIER;
                angleDegrees = 180 - Mathf.Abs(angleDegrees % 360 - 180);
                SetMovement(power, angleDegrees);
                onBounce?.Invoke(bounce, power, angleDegrees);

                bounceEvent = eventManager.Attach(
                    () => movableObject.velocity.y < 0 && movableObject.position.y <= 0,
                    () =>
                    {
                        active = false;
                        bounce = 0;
                        recovering = true;
                        movableObject.acceleration.y = 0;
                        movableObject.velocity.y = 0;
                        movableObject.velocity.x = 0;
                        movableObject.position.y = 0;
                        onFinish?.Invoke();
                        recoverCoroutine = StartCoroutine(RecoverAfterTime());
                    }
                );
            }
        );
    }

    private void SetMovement(float power, float angleDegrees)
    {
        if ((angleDegrees > 0 && angleDegrees < 90) || (angleDegrees > 270 && angleDegrees < 360))
        {
            lookDirection = -1;
        }
        if ((angleDegrees > 90 && angleDegrees < 270))
        {
            lookDirection = 1;
        }
        movableObject.acceleration.y = -gravityAcceleration;
        movableObject.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angleDegrees) * power;
        movableObject.velocity.y = Mathf.Sin(Mathf.Deg2Rad * angleDegrees) * power;
        movableObject.position.y = 0;
    }

    private IEnumerator RecoverAfterTime()
    {
        yield return new WaitForSeconds(knockbackRecoverTime);
        Recover();
    }

    private void Recover()
    {
        recovering = false;
        onRecover?.Invoke();
    }

    public void Stop()
    {
        if (active)
        {
            eventManager.Detach(bounceEvent);
            active = false;
            bounce = 0;
            movableObject.acceleration.y = 0;
            movableObject.velocity.y = 0;
            movableObject.velocity.x = 0;
            movableObject.position.y = 0;
        }
        onFinish?.Invoke();
        if (recovering)
        {
            StopCoroutine(recoverCoroutine);
            Recover();
        }
    }

    public static float GetRelativeDirection(float knockbackDirection, int hitDirection)
    {
        if (hitDirection == 1)
        {
            return knockbackDirection;
        }
        return (180 - knockbackDirection) % 360;
    }
}
