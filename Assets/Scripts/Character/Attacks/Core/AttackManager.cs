using System;
using System.Linq;
using UnityEngine;

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
    public AttackEvents attackEvents;

    /// <value>
    /// Damage transpiler
    /// </value>
    public DamageTranspiler DamageTranspiler { get; private set; }

    private string forgetComboTimeout;

    protected override void Awake()
    {
        base.Awake();
        DamageTranspiler = new DamageTranspiler();
    }

    private void Start()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            // Forward events

            attack.PlayEvents.onPlay += playEvents.onPlay.Invoke;
            attack.attackEvents.onStartAnticipating += () => attackEvents.onStartAnticipating.Invoke();
            attack.attackEvents.onFinishAnticipating += () => attackEvents.onFinishAnticipating.Invoke();
            attack.attackEvents.onStartActive += () => attackEvents.onStartActive.Invoke();
            attack.attackEvents.onFinishActive += () => attackEvents.onFinishActive.Invoke();
            attack.attackEvents.onStartRecovery += () => attackEvents.onStartRecovery.Invoke();
            attack.attackEvents.onFinishRecovery += () => attackEvents.onFinishRecovery.Invoke();
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
        var attackComponents = GetComponents<BaseAttack>();
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
    /// Checks if an attack can be played 
    /// </summary>
    /// <param name="instant">Is the checked attack instant</param>
    /// <returns><c>true</c> if attack can be played</returns>
    public bool CanPlayMove(bool instant = false)
    {
        return !AnyAttack(attack => IsPreventing(attack, instant));
    }
}