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
        OnPlay += StopOtherAttacks;
    }

    public void Start()
    {
        CreateHitDetector();
    }

    protected void PreventWalking(bool freeze)
    {
        OnPlay += () =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            if (freeze)
            {
                MovableObject.velocity = Vector3.zero;
            }
        };
        OnStop += () => EnableBehaviours(typeof(WalkBehaviour));
    }

    private void StopOtherAttacks()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (attack != this)
            {
                attack.Stop();
            }
        }
    }

    protected bool IsTagIncluded(string tag, string group)
    {
        if (tag == group)
        {
            return true;
        }
        return tag.StartsWith(group + ".");
    }

    protected bool IsHittableTag(string tag)
    {
        foreach (string hittableTag in HittableTags)
        {
            if (IsTagIncluded(tag, hittableTag))
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void CreateHitDetector()
    {
        SingleHitDetector hitDetector = new(EventManager, hitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
            {
                HitCallable(hittable);
            }
        });
        OnStart += () => hitDetector.Start();
        OnFinish += () => hitDetector.Stop();
    }

    public override bool CanPlay()
    {
        AttackManager attackManager = GetComponent<AttackManager>();

        return base.CanPlay() 
            && midair == IsPlaying(typeof(JumpBehaviour))
            && AllStopped(typeof(SlideBehaviour), typeof(DodgeBehaviour))
            && !((attackManager.Anticipating || attackManager.Active || (hardRecovery && attackManager.Recovering)) && !(instant && attackManager.IsInterruptable()))
            && ComboCondition();
    }

    protected virtual bool ComboCondition()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return previousAttacks.Count == 0 || previousAttacks.Contains(attackManager.lastAttack);
    }

    protected virtual float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, hittableBehaviour, damage);
    }

    protected override void HitCallable(HittableBehaviour hittableBehaviour)
    {
        float damage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by " + AttackName);
        switch (hitType)
        {
            case HitType.Knockback:
                int hitDirection = (int)Mathf.Sign(hittableBehaviour.MovableObject.position.x - MovableObject.position.x);
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
