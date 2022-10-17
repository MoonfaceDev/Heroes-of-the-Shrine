using System;
using System.Collections.Generic;
using UnityEngine;

public delegate float DamageBonus(BaseAttack attack, HittableBehaviour hittable);

public class AttackManager : PlayableBehaviour
{
    public float maxComboDelay;
    public List<string> hittableTags;
    [HideInInspector] public BaseAttack lastAttack;
    [HideInInspector] public float lastAttackTime;

    public event Action onStart;
    public event Action onFinish;
    public event Action onRecover;

    private List<DamageBonus> damageBonuses;
    private List<DamageBonus> damageMultipliers;

    public override void Awake()
    {
        base.Awake();
        damageBonuses = new();
        damageMultipliers = new();
    }

    public void Start()
    {
        eventManager.Attach(() => true, () =>
        {
            if (lastAttack != null && Time.time - lastAttackTime > maxComboDelay)
            {
                lastAttack = null;
            }
        }, single: false);

        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.onPlay += () => InvokeOnPlay();
            attack.onStart += () => onStart?.Invoke();
            attack.onFinish += () => onFinish?.Invoke();
            attack.onRecover += () => onRecover?.Invoke();
            attack.onStop += () => InvokeOnStop();

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

    public bool anticipating => AnyAttack((attack) => attack.anticipating);

    public bool active => AnyAttack((attack) => attack.active);

    public bool recovering => AnyAttack((attack) => attack.recovering);

    public override bool Playing => AnyAttack((attack) => attack.Playing);

    public bool IsUninterruptable()
    {
        return AnyAttack((attack) => attack.active && !attack.interruptable);
    }

    public override void Stop()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.Stop();
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
