using System;
using System.Collections.Generic;
using UnityEngine;

public delegate float DamageBonus(BaseAttack attack, HittableBehaviour hittable);

public class AttackManager : CharacterBehaviour
{
    public float maxComboDelay;
    public List<string> hittableTags;
    [HideInInspector] public BaseAttack lastAttack;
    [HideInInspector] public float lastAttackTime;

    private List<DamageBonus> damageBonuses;
    private List<DamageBonus> damageMultipliers;

    public override void Awake()
    {
        base.Awake();
        damageBonuses = new();
        damageMultipliers = new();

        eventManager.Attach(() => true, () =>
        {
            if (lastAttack != null && Time.time - lastAttackTime > maxComboDelay)
            {
                lastAttack = null;
            }
        }, single: false);
    }

    public void Start()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.onFinish += () =>
            {
                lastAttack = attack;
                lastAttackTime = Time.time;
            };
        }
    }

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (callback(attack))
            {
                return true;
            }
        }
        return false;
    }

    public bool anticipating
    {
        get => AnyAttack((attack) => attack.anticipating);
    }

    public bool active
    {
        get => AnyAttack((attack) => attack.active);
    }

    public bool recovering
    {
        get => AnyAttack((attack) => attack.recovering);
    }

    public bool attacking
    {
        get => AnyAttack((attack) => attack.attacking);
    }

    public bool CanWalk()
    {
        return !AnyAttack((attack) => attack.attacking && !attack.CanWalk());
    }

    public bool IsUninterruptable()
    {
        return AnyAttack((attack) => attack.active && !attack.interruptable);
    }

    public void Stop()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (attack.attacking)
            {
                attack.Stop();
            }
        }
    }

    public float TranspileDamage(BaseAttack attack, HittableBehaviour hittable, float damage)
    {
        foreach (DamageBonus bonus in damageBonuses)
        {
            damage += bonus(attack, hittable);
        }
        foreach (DamageBonus bonus in damageMultipliers)
        {
            damage *= bonus(attack, hittable);
        }
        return damage;
    }

    public void AttachDamageBonus(DamageBonus bonus)
    {
        damageBonuses.Add(bonus);
    }

    public void DetachDamageBonus(DamageBonus bonus)
    {
        damageBonuses.Remove(bonus);
    }

    public void AttachDamageMultiplier(DamageBonus multiplier)
    {
        damageMultipliers.Add(multiplier);
    }

    public void DetachDamageMultiplier(DamageBonus multiplier)
    {
        damageMultipliers.Remove(multiplier);
    }
}
