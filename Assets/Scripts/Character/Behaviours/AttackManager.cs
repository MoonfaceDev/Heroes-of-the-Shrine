using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public delegate float DamageBonus(BaseAttack attack, IHittable hittable);

public class AttackManager : CharacterBehaviour
{
    public float maxComboDelay;
    public List<string> hittableTags;
    [HideInInspector] public BaseAttack lastAttack;

    public UnityEvent onPlay;
    public UnityEvent onStop;
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
            attack.PlayEvents.onPlay.AddListener(onPlay.Invoke);
            attack.attackEvents.onStartAnticipating.AddListener(() => OnStartAnticipating?.Invoke());
            attack.attackEvents.onFinishAnticipating.AddListener(() => OnFinishAnticipating?.Invoke());
            attack.attackEvents.onStartActive.AddListener(() => OnStartActive?.Invoke());
            attack.attackEvents.onFinishActive.AddListener(() => OnFinishActive?.Invoke());
            attack.attackEvents.onStartRecovery.AddListener(() => OnStartRecovery?.Invoke());
            attack.attackEvents.onFinishRecovery.AddListener(() => OnFinishRecovery?.Invoke());
            attack.PlayEvents.onStop.AddListener(onStop.Invoke);

            // Combo handling

            attack.PlayEvents.onPlay.AddListener(() =>
            {
                Cancel(forgetComboTimeout);
                lastAttack = attack;
            });

            void ForgetComboAction()
            {
                Cancel(forgetComboTimeout);
                forgetComboTimeout = StartTimeout(() => lastAttack = null, maxComboDelay);
            }

            attack.PlayEvents.onStop.AddListener(ForgetComboAction);
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

    public bool Playing => AnyAttack(attack => attack.Playing);

    public bool IsInterruptible()
    {
        return !AnyAttack(attack => attack.Playing && !attack.interruptible);
    }

    public bool CanPlayAttack(bool instant)
    {
        return !((Anticipating || Active || HardRecovering) && !(instant && IsInterruptible()));
    }

    public float TranspileDamage(BaseAttack attack, IHittable hittable, float damage)
    {
        damage += damageBonuses.Sum(bonus => bonus(attack, hittable));
        return damageMultipliers.Aggregate(damage, (current, bonus) => current * bonus(attack, hittable));
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