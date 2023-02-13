using System;
using System.Collections;
using UnityEngine;

public class KnockbackBehaviour : PlayableBehaviour<KnockbackBehaviour.Command>, IForcedBehaviour
{
    public class Command
    {
        public readonly float power;
        public readonly float angleDegrees;

        public Command(float power, float angleDegrees)
        {
            this.power = power;
            this.angleDegrees = angleDegrees;
        }
    }

    private const float SecondBouncePowerMultiplier = 0.2f;

    public float knockbackRecoverTime;

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

    private static readonly Type[] BlockedBehaviours = { typeof(IControlledBehaviour), typeof(StunBehaviour) };

    private static readonly int KnockbackParameter = Animator.StringToHash("knockback");
    private static readonly int RecoveringFromKnockbackParameter = Animator.StringToHash("recoveringFromKnockback");
    private static readonly int BounceParameter = Animator.StringToHash("bounce");

    protected override void DoPlay(Command command)
    {
        Stop();
        StopBehaviours(BlockedBehaviours);
        BlockBehaviours(BlockedBehaviours);

        Active = true;
        Bounce = 1;
        SetMovement(command.power, command.angleDegrees);

        void Land()
        {
            MovableEntity.OnLand -= Land;
            currentLandEvent = null;
            Active = false;
            Bounce = 0;
            Recovering = true;
            MovableEntity.velocity.x = 0;
            OnFinish?.Invoke();
            recoverCoroutine = StartCoroutine(RecoverAfterTime());
        }

        void SecondBounce()
        {
            MovableEntity.OnLand -= SecondBounce;
            Bounce = 2;
            SetMovement(command.power * SecondBouncePowerMultiplier, 180 - Mathf.Abs(command.angleDegrees % 360 - 180));
            MovableEntity.OnLand += Land;
            currentLandEvent = Land;
        }

        MovableEntity.OnLand += SecondBounce;
        currentLandEvent = SecondBounce;
    }

    private void SetMovement(float power, float angleDegrees)
    {
        switch (angleDegrees)
        {
            case > 0 and < 90:
            case > 270 and < 360:
                MovableEntity.rotation = Rotation.Flipped;
                break;
            case > 90 and < 270:
                MovableEntity.rotation = Rotation.Normal;
                break;
        }

        MovableEntity.acceleration.y = -Character.stats.gravityAcceleration;
        MovableEntity.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angleDegrees) * power;
        MovableEntity.velocity.y = Mathf.Sin(Mathf.Deg2Rad * angleDegrees) * power;
        MovableEntity.velocity.z = 0;
    }

    private IEnumerator RecoverAfterTime()
    {
        yield return new WaitForSeconds(knockbackRecoverTime);
        Stop();
    }

    protected override void DoStop()
    {
        UnblockBehaviours(BlockedBehaviours);

        if (Active)
        {
            MovableEntity.OnLand -= currentLandEvent;
            Active = false;
            Bounce = 0;
            MovableEntity.acceleration.y = 0;
            MovableEntity.velocity.y = 0;
            MovableEntity.velocity.x = 0;
            OnFinish?.Invoke();
        }

        if (Recovering)
        {
            StopCoroutine(recoverCoroutine);
            Recovering = false;
            OnRecover?.Invoke();
        }
    }

    public static float GetRelativeDirection(float knockbackAngle, Rotation hitDirection)
    {
        if (hitDirection == Rotation.Normal)
        {
            return knockbackAngle;
        }

        return (180 - knockbackAngle) % 360;
    }
}