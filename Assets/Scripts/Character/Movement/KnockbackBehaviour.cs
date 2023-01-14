using System;
using System.Collections;
using UnityEngine;

public delegate void BounceCallback(int count, float power, float angleDegrees);

public class KnockbackCommand : ICommand
{
    public readonly float power;
    public readonly float angleDegrees;

    public KnockbackCommand(float power, float angleDegrees)
    {
        this.power = power;
        this.angleDegrees = angleDegrees;
    }
}

public class KnockbackBehaviour : ForcedBehaviour<KnockbackCommand>
{
    private const float SecondBouncePowerMultiplier = 0.2f;

    public float knockbackRecoverTime;

    public event BounceCallback OnBounce;
    public event Action OnFinish;
    public event Action OnRecover;

    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(KnockbackParameter, active);
        }
    }

    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(RecoveringFromKnockbackParameter, recovering);
        }
    }

    public int Bounce
    {
        get => bounce;
        private set
        {
            bounce = value;
            Animator.SetInteger(BounceParameter, bounce);
        }
    }

    public override bool Playing => Active || Recovering;

    private bool active;
    private bool recovering;
    private int bounce;
    private Action currentLandEvent;
    private Coroutine recoverCoroutine;

    private static readonly int KnockbackParameter = Animator.StringToHash("knockback");
    private static readonly int RecoveringFromKnockbackParameter = Animator.StringToHash("recoveringFromKnockback");
    private static readonly int BounceParameter = Animator.StringToHash("bounce");

    protected override void DoPlay(KnockbackCommand command)
    {
        StopBehaviours(typeof(IMovementBehaviour), typeof(BaseAttack), typeof(StunBehaviour));

        Active = true;
        Bounce = 1;
        SetMovement(command.power, command.angleDegrees);

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
            SetMovement(command.power * SecondBouncePowerMultiplier, 180 - Mathf.Abs(command.angleDegrees % 360 - 180));
            MovableObject.OnLand += Land;
            currentLandEvent = Land;
            OnBounce?.Invoke(Bounce, command.power, command.angleDegrees);
        }

        MovableObject.OnLand += SecondBounce;
        currentLandEvent = SecondBounce;
        OnBounce?.Invoke(Bounce, command.power, command.angleDegrees);
    }

    private void SetMovement(float power, float angleDegrees)
    {
        switch (angleDegrees)
        {
            case > 0 and < 90:
            case > 270 and < 360:
                MovableObject.rotation = Rotation.Left;
                break;
            case > 90 and < 270:
                MovableObject.rotation = Rotation.Right;
                break;
        }

        MovableObject.acceleration.y = -Character.physicalAttributes.gravityAcceleration;
        MovableObject.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angleDegrees) * power;
        MovableObject.velocity.y = Mathf.Sin(Mathf.Deg2Rad * angleDegrees) * power;
        MovableObject.velocity.z = 0;
    }

    private IEnumerator RecoverAfterTime()
    {
        yield return new WaitForSeconds(knockbackRecoverTime);
        Stop();
    }

    protected override void DoStop()
    {
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
            Recovering = false;
            OnRecover?.Invoke();
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