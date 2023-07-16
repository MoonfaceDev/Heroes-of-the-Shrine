using System;
using System.Linq;
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
    public BaseAttack LastAttack { get; private set; }

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
        InitializeEventForwarding();
        TrackLastAttack();
    }

    private void TrackLastAttack()
    {
        foreach (var attack in GetBehaviours<BaseAttack>())
        {
            attack.PlayEvents.onPlay += () =>
            {
                Cancel(forgetComboTimeout);
                LastAttack = attack;
            };

            attack.PlayEvents.onStop += () =>
            {
                forgetComboTimeout = StartTimeout(() => LastAttack = null, maxComboDelay);
            };
        }
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

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        var attackComponents = GetBehaviours<BaseAttack>();
        return attackComponents.Any(callback);
    }

    /// <value>
    /// Any attack is playing
    /// </value>
    public bool Playing => AnyAttack(attack => attack.Playing);

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