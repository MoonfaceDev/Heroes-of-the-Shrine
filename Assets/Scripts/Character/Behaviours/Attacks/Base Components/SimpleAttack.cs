using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Knockback,
    Stun,
}

public class SimpleAttack : BaseAttack
{
    public List<BaseAttack> previousAttacks;
    public bool midair = false;
    public float anticipateDuration;
    public float activeDuration;
    public float recoveryDuration;
    public Hitbox hitbox;
    public bool overrideDefaultHittableTags = false;
    public List<string> hittableTags;
    public float damage;
    public HitType hitType = HitType.Knockback;
    public float knockbackPower = 0;
    public float knockbackDirection = 0;
    public float stunTime = 0;

    protected virtual List<string> HittableTags
    {
        get => overrideDefaultHittableTags ? hittableTags : GetComponent<AttackManager>().hittableTags;
    }

    public override void Awake()
    {
        base.Awake();
        onAnticipate += StopOtherAttacks;
        CreateHitDetector();
    }

    private void StopOtherAttacks()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (attack != this && attack.attacking)
            {
                attack.Stop();
            }
        }
    }

    protected virtual void CreateHitDetector()
    {
        SingleHitDetector hitDetector = new(eventManager, hitbox, (hittable) =>
        {
            if (HittableTags.Contains(hittable.tag))
            {
                HitCallable(hittable);
            }
        });
        onStart += () => hitDetector.Start();
        void StopHitDetector() => hitDetector.Stop();
        onFinish += StopHitDetector;
        onStop += StopHitDetector;
    }

    public override bool CanWalk()
    {
        return false;
    }

    public override bool CanAttack()
    {
        JumpBehaviour jumpBehaviour = GetComponent<JumpBehaviour>();
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        DodgeBehaviour dodgeBehaviour = GetComponent<DodgeBehaviour>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();

        return midair == (jumpBehaviour && jumpBehaviour.jump)
            && !(slideBehaviour && slideBehaviour.slide)
            && !(dodgeBehaviour && dodgeBehaviour.dodge)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager.anticipating || attackManager.active) && !(instant && !attackManager.IsUninterruptable())
            && ComboCondition();
    }

    protected virtual bool ComboCondition()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return previousAttacks.Count == 0 || previousAttacks.Contains(attackManager.lastAttack);
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
                int hitDirection = (int)Mathf.Sign(hittableBehaviour.movableObject.position.x - movableObject.position.x);
                hittableBehaviour.Knockback(damage, knockbackPower, KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection));
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
