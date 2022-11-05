using System;
using System.Collections;
using UnityEngine;

public delegate void OnBounce(int count, float power, float angleDegrees);

public class KnockbackBehaviour : ForcedBehaviour
{
    public static float SECOND_BOUNCE_POWER_MULTIPLIER = 0.2f;

    public float knockbackRecoverTime;

    public event OnBounce OnBounce;
    public event Action OnFinish;
    public event Action OnRecover;

    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool("knockback", active);
        }
    }
    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool("recoveringFromKnockback", recovering);
        }
    }
    public int Bounce
    {
        get => bounce;
        private set
        {
            bounce = value;
            Animator.SetInteger("bounce", bounce);
        }
    }

    public override bool Playing => Active || Recovering;

    private bool active;
    private bool recovering;
    private int bounce;
    private Action currentLandEvent;
    private Coroutine recoverCoroutine;

    public void Play(float power, float angleDegrees)
    {
        if (!CanPlay())
        {
            return;
        }

        StopBehaviours(typeof(BaseMovementBehaviour), typeof(AttackManager), typeof(StunBehaviour));

        Active = true;
        InvokeOnPlay();
        Bounce = 1;
        SetMovement(power, angleDegrees);

        void Land()
        {
            MovableObject.OnLand -= Land;
            currentLandEvent = null;
            Active = false;
            Bounce = 0;
            Recovering = true;
            MovableObject.velocity.x = 0;
            OnFinish?.Invoke();
            recoverCoroutine = StartCoroutine(RecoverAfterTime());
        }

        void SecondBounce()
        {
            MovableObject.OnLand -= SecondBounce;
            Bounce = 2;
            power *= SECOND_BOUNCE_POWER_MULTIPLIER;
            angleDegrees = 180 - Mathf.Abs(angleDegrees % 360 - 180);
            SetMovement(power, angleDegrees);
            MovableObject.OnLand += Land;
            currentLandEvent = Land;
            OnBounce?.Invoke(Bounce, power, angleDegrees);
        }

        MovableObject.OnLand += SecondBounce;
        currentLandEvent = SecondBounce;
        OnBounce?.Invoke(Bounce, power, angleDegrees);
    }

    private void SetMovement(float power, float angleDegrees)
    {
        if ((angleDegrees > 0 && angleDegrees < 90) || (angleDegrees > 270 && angleDegrees < 360))
        {
            LookDirection = -1;
        }
        if (angleDegrees > 90 && angleDegrees < 270)
        {
            LookDirection = 1;
        }
        MovableObject.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
        MovableObject.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angleDegrees) * power;
        MovableObject.velocity.y = Mathf.Sin(Mathf.Deg2Rad * angleDegrees) * power;
    }

    private IEnumerator RecoverAfterTime()
    {
        yield return new WaitForSeconds(knockbackRecoverTime);
        Recover();
    }

    private void Recover()
    {
        Recovering = false;
        OnRecover?.Invoke();
        InvokeOnStop();
    }

    public override void Stop()
    {
        if (Playing)
        {
            InvokeOnStop();
        }
        if (Active)
        {
            MovableObject.OnLand -= currentLandEvent;
            Active = false;
            Bounce = 0;
            MovableObject.acceleration.y = 0;
            MovableObject.velocity.y = 0;
            MovableObject.velocity.x = 0;
            OnFinish?.Invoke();
        }
        if (Recovering)
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
