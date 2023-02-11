using System;
using System.Linq;
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
    [HideInInspector] public BaseAttack lastAttack;

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

    private string forgetComboTimeout;

    protected override void Awake()
    {
        base.Awake();

        damageTranspiler = new DamageTranspiler();
        damageTranspiler.Add((_, _, value) => Character.stats.damageMultiplier * value);

        knockbackPowerTranspiler = new KnockbackPowerTranspiler();
        knockbackPowerTranspiler.Add((_, _, value) => Character.stats.knockbackPowerMultiplier * value);
    }

    private void Start()
    {
        var attackComponents = GetBehaviours<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            // Forward events

            attack.PlayEvents.onPlay += playEvents.onPlay.Invoke;
            attack.phaseEvents.onStartAnticipating += () => phaseEvents.onStartAnticipating.Invoke();
            attack.phaseEvents.onFinishAnticipating += () => phaseEvents.onFinishAnticipating.Invoke();
            attack.phaseEvents.onStartActive += () => phaseEvents.onStartActive.Invoke();
            attack.phaseEvents.onFinishActive += () => phaseEvents.onFinishActive.Invoke();
            attack.phaseEvents.onStartRecovery += () => phaseEvents.onStartRecovery.Invoke();
            attack.phaseEvents.onFinishRecovery += () => phaseEvents.onFinishRecovery.Invoke();
            attack.PlayEvents.onStop += playEvents.onStop.Invoke;

            // Combo handling

            attack.PlayEvents.onPlay += () =>
            {
                Cancel(forgetComboTimeout);
                lastAttack = attack;
            };

            attack.PlayEvents.onStop += () =>
            {
                Cancel(forgetComboTimeout);
                forgetComboTimeout = StartTimeout(() => lastAttack = null, maxComboDelay);
            };
        }
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

    private static bool IsPreventing(BaseAttack attack, bool instant)
    {
        return (attack.Anticipating || attack.Active || (attack.hardRecovery && attack.Recovering)) &&
               !(instant && attack.interruptible);
    }

    /// <summary>
    /// Checks if a move can be played 
    /// </summary>
    /// <param name="instant">Is the checked move instant</param>
    /// <returns><c>true</c> if move can be played</returns>
    public bool CanPlayMove(bool instant = false)
    {
        return !AnyAttack(attack => IsPreventing(attack, instant));
    }
}