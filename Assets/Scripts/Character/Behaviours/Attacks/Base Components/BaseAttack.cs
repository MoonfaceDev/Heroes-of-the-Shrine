using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains start and finish events for all attack phases
/// </summary>
[Serializable]
public class AttackEvents
{
    /// <value>
    /// Attack anticipation has started
    /// </value>
    public UnityEvent onStartAnticipating;

    /// <value>
    /// Attack anticipation has finished, also fires if the attack was stopped while in anticipation
    /// </value>
    public UnityEvent onFinishAnticipating;

    /// <value>
    /// Attack active phase has started
    /// </value>
    public UnityEvent onStartActive;

    /// <value>
    /// Attack active phase has finished, also fires if the attack was stopped while in active phase
    /// </value>
    public UnityEvent onFinishActive;

    /// <value>
    /// Attack recovery has started
    /// </value>
    public UnityEvent onStartRecovery;

    /// <value>
    /// Attack recovery has finished, also fires if the attack was stopped while in recovery
    /// </value>
    public UnityEvent onFinishRecovery;
}

public class BaseAttackCommand : ICommand
{
}

/// <summary>
/// Base class for all attacks. Most attacks should derive from <see cref="SimpleAttack"/>, which has more members and helper methods.
/// </summary>
[RequireComponent(typeof(AttackManager))]
public abstract class BaseAttack : PlayableBehaviour<BaseAttackCommand>
{
    /// <value>
    /// If true, the attack can be played while another interruptible attack is playing.
    /// </value>
    public bool instant;

    /// <value>
    /// If true, an instant attack can replace it while this attack is playing.
    /// </value>
    public bool interruptible = true;

    /// <value>
    /// If <c>true</c>, The attack cannot be interrupted while <see cref="Recovering"/> is true.
    /// </value>
    public bool hardRecovery;

    /// <value>
    /// Name of the attack, in the format <c>CamelCase</c>.
    /// Used in combo system, and in animator parameters.
    /// </value>
    public string AttackName => GetType().Name;

    /// <value>
    /// Attack is anticipating.
    /// It also sets the animator parameter: <c>{attackName}-anticipating</c>.
    /// </value>
    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(AttackName + "-anticipating", anticipating);
            (value ? attackEvents.onStartAnticipating : attackEvents.onFinishAnticipating).Invoke();
        }
    }

    /// <value>
    /// Attack is active.
    /// It also sets the animator parameter: <c>{attackName}-active</c>.
    /// </value>
    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(AttackName + "-active", active);
            (value ? attackEvents.onStartActive : attackEvents.onFinishActive).Invoke();
        }
    }

    /// <value>
    /// Attack is recovering.
    /// It also sets the animator parameter: <c>{attackName}-recovering</c>.
    /// </value>
    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(AttackName + "-recovering", recovering);
            (value ? attackEvents.onStartRecovery : attackEvents.onFinishRecovery).Invoke();
        }
    }

    /// <value>
    /// Attack is playing.
    /// True if attack is either <see cref="Anticipating"/>, <see cref="Active"/> or <see cref="Recovering"/>.
    /// </value>
    public override bool Playing => Anticipating || Active || Recovering;

    /// <value>
    /// General attack events
    /// </value>
    public AttackEvents attackEvents;

    private bool anticipating;
    private bool active;
    private bool recovering;

    private Coroutine attackFlowCoroutine;

    /// <summary>
    /// Coroutine played while attack is <see cref="Anticipating"/>.
    /// When the coroutine finishes, the anticipation finishes.
    /// </summary>
    /// <returns>Started coroutine.</returns>
    protected abstract IEnumerator AnticipateCoroutine();

    /// <summary>
    /// Coroutine played while attack is <see cref="Active"/>.
    /// When the coroutine finishes, the active phase finishes.
    /// </summary>
    /// <returns>Started coroutine.</returns>
    protected abstract IEnumerator ActiveCoroutine();

    /// <summary>
    /// Coroutine played while attack is <see cref="Recovering"/>.
    /// When the coroutine finishes, the recovery finishes.
    /// </summary>
    /// <returns>Started coroutine.</returns>
    protected abstract IEnumerator RecoveryCoroutine();

    /// <summary>
    /// Tells if the attack can be played.
    /// By default, any attack can be played if it is <see cref="CharacterBehaviour.Enabled"/>, and the character is not under knockback or stunned.
    /// Override to add more conditions that attack requires.
    /// </summary>
    /// <returns><c>true</c> if the attack can be played</returns>
    public override bool CanPlay(BaseAttackCommand command)
    {
        return base.CanPlay(command) && !IsPlaying<KnockbackBehaviour>() && !IsPlaying<StunBehaviour>();
    }

    /// <summary>
    /// Play the attack phases
    /// </summary>
    protected override void DoPlay(BaseAttackCommand command)
    {
        attackFlowCoroutine = StartCoroutine(AttackFlow());
    }

    private IEnumerator AttackFlow()
    {
        Anticipating = true;
        yield return AnticipateCoroutine();
        Anticipating = false;
        Active = true;
        yield return ActiveCoroutine();
        Active = false;
        Recovering = true;
        yield return RecoveryCoroutine();
        Stop();
    }

    /// <summary>
    /// Stops the attack immediately in any of its phases.
    /// Can be safely called even if attack is not currently playing, as it will do nothing.
    /// </summary>
    protected override void DoStop()
    {
        StopCoroutine(attackFlowCoroutine);

        if (Anticipating)
        {
            Anticipating = false;
            attackEvents.onFinishAnticipating.Invoke();
        }

        if (Active)
        {
            Active = false;
            attackEvents.onFinishActive.Invoke();
        }

        if (Recovering)
        {
            Recovering = false;
            attackEvents.onFinishRecovery.Invoke();
        }
    }
}