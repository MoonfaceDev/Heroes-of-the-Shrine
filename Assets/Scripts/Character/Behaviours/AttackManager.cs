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

    public event Action OnStartAnticipating;
    public event Action OnFinishAnticipating;
    public event Action OnStartActive;
    public event Action OnFinishActive;
    public event Action OnStartRecovery;
    public event Action OnFinishRecovery;

    private EventListener forgetComboEvent;
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
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            // Forward events
            attack.OnPlay += () => InvokeOnPlay();
            attack.OnStartAnticipating += () => OnStartAnticipating?.Invoke();
            attack.OnFinishAnticipating += () => OnFinishAnticipating?.Invoke();
            attack.OnStartActive+= () => OnStartActive?.Invoke();
            attack.OnFinishActive += () => OnFinishActive?.Invoke();
            attack.OnStartRecovery += () => OnStartRecovery?.Invoke();
            attack.OnFinishRecovery += () => OnFinishRecovery?.Invoke();
            attack.OnStop += () => InvokeOnStop();

            // Combo handling

            attack.OnPlay += () =>
            {
                EventManager.Detach(forgetComboEvent);
                lastAttack = attack;
            };

            void ForgetComboAction()
            {
                EventManager.Detach(forgetComboEvent);
                forgetComboEvent = EventManager.StartTimeout(() =>
                {
                    lastAttack = null;
                }, maxComboDelay);
            }

            attack.OnFinishRecovery += ForgetComboAction;
            attack.OnStop += ForgetComboAction;
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

    public bool Anticipating => AnyAttack((attack) => attack.Anticipating);

    public bool Active => AnyAttack((attack) => attack.Active);

    public bool Recovering => AnyAttack((attack) => attack.Recovering);

    public bool HardRecovrting => AnyAttack((attack) => attack.hardRecovery && attack.Recovering);

    public override bool Playing => AnyAttack((attack) => attack.Playing);

    public bool IsInterruptable()
    {
        return !AnyAttack((attack) => attack.Playing && !attack.interruptable);
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
