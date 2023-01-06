using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate float DamageBonus(BaseAttack attack, Character character);

public class AttackManager : PlayableBehaviour
{
    public float maxComboDelay;
    public List<string> hittableTags;
    [HideInInspector] public BaseAttack lastAttack;

    public event Action OnStartAnticipating;
    public event Action OnFinishAnticipating;
    public event Action OnStartActive;
    public event Action OnFinishActive;
    public event Action OnStartRecovery;
    public event Action OnFinishRecovery;

    private string forgetComboTimeout;
    private List<DamageBonus> damageBonuses;
    private List<DamageBonus> damageMultipliers;

    public override void Awake()
    {
        base.Awake();
        damageBonuses = new List<DamageBonus>();
        damageMultipliers = new List<DamageBonus>();
    }

    public void Start()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            // Forward events
            attack.onPlay.AddListener(onPlay.Invoke);
            attack.generalEvents.onStartAnticipating.AddListener(() => OnStartAnticipating?.Invoke());
            attack.generalEvents.onFinishAnticipating.AddListener(() => OnFinishAnticipating?.Invoke());
            attack.generalEvents.onStartActive.AddListener(() => OnStartActive?.Invoke());
            attack.generalEvents.onFinishActive.AddListener(() => OnFinishActive?.Invoke());
            attack.generalEvents.onStartRecovery.AddListener(() => OnStartRecovery?.Invoke());
            attack.generalEvents.onFinishRecovery.AddListener(() => OnFinishRecovery?.Invoke());
            attack.onStop.AddListener(onStop.Invoke);

            // Combo handling

            attack.onPlay.AddListener(() =>
            {
                Cancel(forgetComboTimeout);
                lastAttack = attack;
            });

            void ForgetComboAction()
            {
                Cancel(forgetComboTimeout);
                forgetComboTimeout = StartTimeout(() => lastAttack = null, maxComboDelay);
            }

            attack.onStop.AddListener(ForgetComboAction);
        }
    }

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        var attackComponents = GetComponents<BaseAttack>();
        return attackComponents.Any(callback);
    }

    public bool Anticipating => AnyAttack(attack => attack.Anticipating);

    public bool Active => AnyAttack(attack => attack.Active);

    public bool Recovering => AnyAttack(attack => attack.Recovering);

    public bool HardRecovering => AnyAttack(attack => attack.hardRecovery && attack.Recovering);

    public override bool Playing => AnyAttack(attack => attack.Playing);

    public bool IsInterruptible()
    {
        return !AnyAttack(attack => attack.Playing && !attack.interruptible);
    }

    public override void Stop()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            attack.Stop();
        }
    }

    public float TranspileDamage(BaseAttack attack, Character character, float damage)
    {
        damage += damageBonuses.Sum(bonus => bonus(attack, character));
        return damageMultipliers.Aggregate(damage, (current, bonus) => current * bonus(attack, character));
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