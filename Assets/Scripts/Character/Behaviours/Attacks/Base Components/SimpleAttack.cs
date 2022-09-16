using System.Collections;
using UnityEngine;

public enum HitType
{
    Knockback,
    Stun,
}

public class SimpleAttack : BaseAttack
{
    public string previousAttackName;
    public bool midair = false;
    public float anticipateDuration;
    public float activeDuration;
    public float recoveryDuration;
    public float damage;
    public HitType hitType = HitType.Knockback;
    public float knockbackPower = 0;
    public float knockbackDirection = 0;
    public float stunTime = 0;

    protected override bool CanAttack()
    {
        JumpBehaviour jumpBehaviour = GetComponent<JumpBehaviour>();
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();

        return midair == (jumpBehaviour && jumpBehaviour.jump)
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager.anticipating || attackManager.active) && !(instant && !attackManager.IsUninterruptable())
            && ComboCondition();
    }

    protected virtual bool ComboCondition()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return attackManager.lastAttack == previousAttackName;
    }

    protected virtual float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return damage;
    }

    protected override void HitCallable(HittableBehaviour hittableBehaviour)
    {
        float damage = CalculateDamage(hittableBehaviour);
        switch (hitType)
        {
            case HitType.Knockback:
                hittableBehaviour.Knockback(damage, knockbackPower, KnockbackBehaviour.GetRelativeDirection(knockbackDirection, lookDirection));
                break;
            case HitType.Stun:
                hittableBehaviour.Stun(damage, stunTime);
                break;
        }
    }

    protected override IEnumerator AnticipateCoroutine()
    {
        yield return new WaitForSeconds(anticipateDuration);
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitForSeconds(activeDuration);
    }

    protected override IEnumerator RecoveryCoroutine()
    {
        yield return new WaitForSeconds(recoveryDuration);
    }
}
