using System;
using System.Collections.Generic;
using System.Linq;
using ExtEvents;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Behaviour that runs operation on all attacks
/// </summary>
public class AttackManager : CharacterBehaviour
{
    /// <value>
    /// Time after attack finished, that it is removed from combo history
    /// </value>
    public float maxComboDelay;

    /// <value>
    /// Tags of objects that can get hit by character's attacks
    /// </value>
    public Tags hittableTags;

    /// <summary>
    /// Last attack dealt
    /// </summary>
    public BaseAttack LastAttack => comboAttacks.LastOrDefault();

    /// <value>
    /// Attacks play and stop events. Whenever any attack starts or stops, these events are invoked.
    /// </value>
    public PlayEvents playEvents;

    /// <value>
    /// General attack events. These events are invoked whenever a matching event is invoked in any attack.
    /// </value>
    [FormerlySerializedAs("attackEvents")] public PhaseEvents phaseEvents;

    /// <value>
    /// Hit damage transpiler
    /// </value>
    public DamageTranspiler damageTranspiler;

    /// <value>
    /// Hit knockback power transpiler
    /// </value>
    public KnockbackPowerTranspiler knockbackPowerTranspiler;

    /// <value>
    /// Combo was cut in the middle
    /// </value>
    [SerializeField] public ExtEvent onComboCut;

    /// <value>
    /// Attack was blocked
    /// </value>
    [SerializeField] public ExtEvent<Hit> onBlock;

    /// <value>
    /// Entire combo was blocked
    /// </value>
    [SerializeField] public ExtEvent onComboBlock;

    private string forgetComboTimeout;
    private List<BaseAttack> comboAttacks;
    private List<BaseAttack> comboBlocks;

    protected override void Awake()
    {
        base.Awake();

        damageTranspiler = new DamageTranspiler();
        damageTranspiler.Add((_, _, value) => Character.stats.damageMultiplier * value);

        knockbackPowerTranspiler = new KnockbackPowerTranspiler();
        knockbackPowerTranspiler.Add((_, _, value) => Character.stats.knockbackPowerMultiplier * value);

        comboAttacks = new List<BaseAttack>();
        comboBlocks = new List<BaseAttack>();
    }

    private void Start()
    {
        InitializeEventForwarding();
        InitializeCombos();
        InitializeBlocksCombo();
    }

    private void InitializeEventForwarding()
    {
        foreach (var attack in GetBehaviours<BaseAttack>())
        {
            attack.PlayEvents.onPlay += playEvents.onPlay.Invoke;
            attack.phaseEvents.onStartAnticipating += () => phaseEvents.onStartAnticipating.Invoke();
            attack.phaseEvents.onFinishAnticipating += () => phaseEvents.onFinishAnticipating.Invoke();
            attack.phaseEvents.onStartActive += () => phaseEvents.onStartActive.Invoke();
            attack.phaseEvents.onFinishActive += () => phaseEvents.onFinishActive.Invoke();
            attack.phaseEvents.onStartRecovery += () => phaseEvents.onStartRecovery.Invoke();
            attack.phaseEvents.onFinishRecovery += () => phaseEvents.onFinishRecovery.Invoke();
            attack.PlayEvents.onStop += playEvents.onStop.Invoke;
        }
    }

    private void InitializeCombos()
    {
        foreach (var attack in GetBehaviours<BaseAttack>())
        {
            attack.PlayEvents.onPlay += () =>
            {
                if (!attack.previousAttacks.Contains(comboAttacks.LastOrDefault()))
                {
                    comboAttacks.Clear();
                }
                
                Cancel(forgetComboTimeout);
                comboAttacks.Add(attack);
            };

            attack.PlayEvents.onStop += () =>
            {
                Cancel(forgetComboTimeout);
                forgetComboTimeout = StartTimeout(() =>
                {
                    comboAttacks.Clear();
                    onComboCut.Invoke();
                }, maxComboDelay);
            };
        }
    }

    private void InitializeBlocksCombo()
    {
        onBlock += hit =>
        {
            var attack = hit.source;
            comboBlocks.Add(attack);

            if (!IsFinalAttack(attack)) return;

            if (comboBlocks.Count == comboAttacks.Count)
            {
                onComboBlock.Invoke();
            }

            comboBlocks.Clear();
        };

        onComboCut += comboBlocks.Clear;
    }

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        var attackComponents = GetBehaviours<BaseAttack>();
        return attackComponents.Any(callback);
    }

    /// <value>
    /// Any attack is playing
    /// </value>
    public bool Playing => AnyAttack(attack => attack.Playing);

    private bool IsFinalAttack(BaseAttack attack)
    {
        return !AnyAttack(other => other.previousAttacks.Contains(attack));
    }

    private static bool IsPreventing(BaseAttack attack)
    {
        return attack.Anticipating || attack.Active || (attack.hardRecovery && attack.Recovering);
    }

    /// <summary>
    /// Checks if any attack is playing. If attack is recovering, and it doesn't have a <c>hardRecovery</c>, it will not
    /// prevent an attack
    /// </summary>
    /// <returns><c>true</c> if attack can be played</returns>
    public bool CanPlayAttack()
    {
        return !AnyAttack(IsPreventing);
    }
}