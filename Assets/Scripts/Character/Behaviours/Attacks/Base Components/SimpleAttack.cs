using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HitType
{
    Knockback,
    Stun
}

public class SimpleAttack : BaseAttack
{
    public List<BaseAttack> previousAttacks;
    public bool midair;
    public float anticipateDuration;
    public float activeDuration;
    public float recoveryDuration;
    public Hitbox hitbox;
    public bool overrideDefaultHittableTags;
    public List<string> hittableTags;
    public float damage;
    public HitType hitType = HitType.Knockback;
    public float knockbackPower;
    public float knockbackDirection;
    public float stunTime;

    private IEnumerable<string> HittableTags => overrideDefaultHittableTags ? hittableTags : GetComponent<AttackManager>().hittableTags;

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
        OnFinishRecovery += () => EnableBehaviours(typeof(WalkBehaviour));
        OnStop += () => EnableBehaviours(typeof(WalkBehaviour));
    }

    private void StopOtherAttacks()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            if (attack != this)
            {
                attack.Stop();
            }
        }
    }

    private static bool IsTagIncluded(string testedTag, string group)
    {
        return testedTag == group || testedTag.StartsWith(group + ".");
    }

    protected bool IsHittableTag(string testedTag)
    {
        return HittableTags.Any(hittableTag => IsTagIncluded(testedTag, hittableTag));
    }

    protected virtual void CreateHitDetector()
    {
        SingleHitDetector hitDetector = new(EventManager, hitbox, hittable =>
        {
            if (!IsHittableTag(hittable.tag)) return;
            if (hittable.CanGetHit())
            {
                hitbox.PlayParticles();
            }
            HitCallable(hittable);
        });
        OnStartActive += () => hitDetector.Start();
        OnFinishActive += () => hitDetector.Stop();
    }

    public override bool CanPlay()
    {
        var attackManager = GetComponent<AttackManager>();

        return base.CanPlay() 
            && midair == IsPlaying(typeof(JumpBehaviour))
            && AllStopped(typeof(SlideBehaviour), typeof(DodgeBehaviour))
            && !((attackManager.Anticipating || attackManager.Active || attackManager.HardRecovering) && !(instant && attackManager.IsInterruptible()))
            && ComboCondition();
    }

    private bool ComboCondition()
    {
        var attackManager = GetComponent<AttackManager>();
        return previousAttacks.Count == 0 || previousAttacks.Contains(attackManager.lastAttack);
    }

    protected virtual float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, hittableBehaviour, damage);
    }

    protected virtual void HitCallable(HittableBehaviour hittableBehaviour)
    {
        var processedDamage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by " + AttackName);
        switch (hitType)
        {
            case HitType.Knockback:
                int hitDirection = (int)Mathf.Sign(hittableBehaviour.MovableObject.WorldPosition.x - MovableObject.WorldPosition.x);
                hittableBehaviour.Knockback(processedDamage, knockbackPower, KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection));
                break;
            case HitType.Stun:
                hittableBehaviour.Stun(processedDamage, stunTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
