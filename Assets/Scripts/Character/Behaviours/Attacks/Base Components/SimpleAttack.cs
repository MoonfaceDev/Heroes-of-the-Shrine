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

        return !(jumpBehaviour && (jumpBehaviour.anticipating || jumpBehaviour.active || jumpBehaviour.recovering))
            && !(slideBehaviour && slideBehaviour.active)
            && !(knockbackBehaviour && (knockbackBehaviour.active || knockbackBehaviour.recovering))
            && !(stunBehaviour && stunBehaviour.active)
            && !(attackManager && attackManager.attacking && !(instant && !attackManager.IsUninterruptable()));
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
                hittableBehaviour.Knockback(damage, knockbackPower, knockbackDirection);
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
