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

    [SerializeField] private PlayEvents playEvents;

    /// <value>
    /// Attacks play and stop events. Whenever any attack starts or stops, these events are invoked.
    /// </value>
    public PlayEvents PlayEvents => playEvents;

    /// <value>
    /// General attack events. These events are invoked whenever a matching event is invoked in any attack.
    /// </value>
    public AttackEvents attackEvents;

    /// <value>
    /// Damage transpiler
    /// </value>
    public DamageTranspiler DamageTranspiler { get; private set; }

    private string forgetComboTimeout;

    public override void Awake()
    {
        base.Awake();
        DamageTranspiler = new DamageTranspiler();
    }

    public void Start()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            // Forward events

            attack.PlayEvents.onPlay.AddListener(playEvents.onPlay.Invoke);
            attack.attackEvents.onStartAnticipating.AddListener(() => attackEvents.onStartAnticipating.Invoke());
            attack.attackEvents.onFinishAnticipating.AddListener(() => attackEvents.onFinishAnticipating.Invoke());
            attack.attackEvents.onStartActive.AddListener(() => attackEvents.onStartActive.Invoke());
            attack.attackEvents.onFinishActive.AddListener(() => attackEvents.onFinishActive.Invoke());
            attack.attackEvents.onStartRecovery.AddListener(() => attackEvents.onStartRecovery.Invoke());
            attack.attackEvents.onFinishRecovery.AddListener(() => attackEvents.onFinishRecovery.Invoke());
            attack.PlayEvents.onStop.AddListener(PlayEvents.onStop.Invoke);

            // Combo handling

            attack.PlayEvents.onPlay.AddListener(() =>
            {
                Cancel(forgetComboTimeout);
                lastAttack = attack;
            });

            attack.PlayEvents.onStop.AddListener(() =>
            {
                Cancel(forgetComboTimeout);
                forgetComboTimeout = StartTimeout(() => lastAttack = null, maxComboDelay);
            });
        }
    }

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        var attackComponents = GetComponents<BaseAttack>();
        return attackComponents.Any(callback);
    }

    /// <value>
    /// Any attack is anticipating
    /// </value>
    public bool Anticipating => AnyAttack(attack => attack.Anticipating);

    /// <value>
    /// Any attack is active
    /// </value>
    public bool Active => AnyAttack(attack => attack.Active);

    /// <value>
    /// Any attack is recovering
    /// </value>
    public bool Recovering => AnyAttack(attack => attack.Recovering);

    /// <value>
    /// Any attack is recovering and its <see cref="BaseAttack.hardRecovery"/> value is <c>true</c>
    /// </value>
    public bool HardRecovering => AnyAttack(attack => attack.hardRecovery && attack.Recovering);

    /// <value>
    /// Any attack is playing
    /// </value>
    public bool Playing => AnyAttack(attack => attack.Playing);

    /// <returns><c>true</c> if there aren't any uninterruptible attacks playing</returns>
    public bool IsInterruptible()
    {
        return !AnyAttack(attack => attack.Playing && !attack.interruptible);
    }

    /// <summary>
    /// Checks if an attack can be played 
    /// </summary>
    /// <param name="instant">Is the checked attack instant</param>
    /// <returns><c>true</c> if attack can be played</returns>
    public bool CanPlayAttack(bool instant)
    {
        return !((Anticipating || Active || HardRecovering) && !(instant && IsInterruptible()));
    }
}