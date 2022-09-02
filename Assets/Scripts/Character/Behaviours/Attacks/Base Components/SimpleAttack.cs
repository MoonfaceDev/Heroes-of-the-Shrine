using System.Collections;
using UnityEngine;

public enum HitType
{
    Knockback,
    Stun,
}

public class SimpleAttack : BaseAttack
{
    public float anticipateDuration;
    public float activeDuration;
    public float recoveryDuration;
    public Hitbox hitbox;
    public float damage;
    public HitType hitType = HitType.Knockback;
    public float knockbackPower = 0;
    public float knockbackDirection = 0;
    public float stunTime = 0;

    public override bool anticipating
    {
        get => _anticipating;
        protected set
        {
            _anticipating = value;
            animator.SetBool(attackName + "-anticipating", _anticipating);
        }
    }
    public override bool active
    {
        get => _active;
        protected set
        {
            _active = value;
            animator.SetBool(attackName + "-active", _active);
        }
    }
    public override bool recovering
    {
        get => _recovering;
        protected set
        {
            _recovering = value;
            animator.SetBool(attackName + "-recovering", _recovering);
        }
    }

    private bool _anticipating;
    private bool _active;
    private bool _recovering;
    protected Coroutine attackCoroutine;
    protected SingleHitDetector hitDetector;

    public override void Attack()
    {
        if (!CanAttack())
        {
            return;
        }
        WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
        if (walkBehaviour)
        {
            walkBehaviour.Stop();
        }
        attackCoroutine = StartCoroutine(AttackFlow());
        hitDetector = new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (ShouldTarget(hittableBehaviour))
            {
                OnHit(hittableBehaviour);
            }
        });
    }

    protected IEnumerator AttackFlow()
    {
        anticipating = true;
        InvokeOnAnticipate();
        yield return new WaitForSeconds(anticipateDuration);

        anticipating = false;
        active = true;
        InvokeOnStart();
        hitDetector.Start();
        yield return new WaitForSeconds(activeDuration);

        hitDetector.Stop();
        active = false;
        recovering = true;
        InvokeOnFinish();
        yield return new WaitForSeconds(recoveryDuration);

        recovering = false;
        InvokeOnRecover();
    }

    public override bool CanAttack()
    {
        JumpBehaviour jumpBehaviour = GetComponent<JumpBehaviour>();
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();

        return !(jumpBehaviour && (jumpBehaviour.anticipating || jumpBehaviour.active || jumpBehaviour.recovering))
            && !(slideBehaviour && slideBehaviour.active)
            && !(knockbackBehaviour && (knockbackBehaviour.active || knockbackBehaviour.recovering))
            && !(stunBehaviour && stunBehaviour.active)
            && !(attackManager && attackManager.attacking && !(instant && !attackManager.IsUninterruptable()));
    }

    public override bool ShouldTarget(HittableBehaviour hittableBehaviour)
    {
        return hittableBehaviour;
    }

    public override void OnHit(HittableBehaviour hittableBehaviour)
    {
        switch (hitType)
        {
            case HitType.Knockback:
                hittableBehaviour.Knockback(damage, knockbackPower, knockbackDirection);
                break;
            case HitType.Stun:
                hittableBehaviour.Stun(damage, stunTime);
                break;
        }
    }
}
