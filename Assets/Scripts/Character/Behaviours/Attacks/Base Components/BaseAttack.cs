using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Attack could not be played
/// </summary>
public class CannotAttackException : Exception
{
}

[RequireComponent(typeof(AttackManager))]
public abstract class BaseAttack : PlayableBehaviour
{
    /// <value>
    /// If true, the attack can be played while another interruptible attack is playing.
    /// </value>
    public bool instant;

    /// <value>
    /// If true, an instant attack can replace it while this attack is playing.
    /// </value>
    [FormerlySerializedAs("interruptable")]
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
            (value ? OnStartAnticipating : OnFinishAnticipating)?.Invoke();
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
            (value ? OnStartActive : OnFinishActive)?.Invoke();
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
            (value ? OnStartRecovery : OnFinishRecovery)?.Invoke();
        }
    }

    /// <value>
    /// Attack is playing.
    /// True if attack is either <see cref="Anticipating"/>, <see cref="Active"/> or <see cref="Recovering"/>.
    /// </value>
    public override bool Playing => Anticipating || Active || Recovering;

    /// <value>
    /// Attack anticipation has started.
    /// </value>
    public event Action OnStartAnticipating;

    /// <value>
    /// Attack anticipation has finished.
    /// Also fires if the attack was stopped while in anticipation.
    /// </value>
    public event Action OnFinishAnticipating;

    /// <value>
    /// Attack active phase has started.
    /// </value>
    public event Action OnStartActive;

    /// <value>
    /// Attack active phase has finished.
    /// Also fires if the attack was stopped while in active phase.
    /// </value>
    public event Action OnFinishActive;

    /// <value>
    /// Attack recovery has started.
    /// </value>
    public event Action OnStartRecovery;

    /// <value>
    /// Attack recovery has finished.
    /// Also fires if the attack was stopped while in recovery.
    /// </value>
    public event Action OnFinishRecovery;

    private bool anticipating;
    private bool active;
    private bool recovering;

    protected AttackManager AttackManager
    {
        get
        {
            if (attackManager == null)
            {
                attackManager = GetComponent<AttackManager>();
            }

            return attackManager;
        }
    }

    private AttackManager attackManager;

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
    public override bool CanPlay()
    {
        return base.CanPlay() && AllStopped(typeof(KnockbackBehaviour), typeof(StunBehaviour));
    }

    /// <summary>
    /// Play the attack phases.
    /// </summary>
    /// <exception cref="CannotAttackException">Thrown if attack cannot be played.</exception>
    public void Play()
    {
        if (!CanPlay())
        {
            throw new CannotAttackException();
        }

        InvokeOnPlay();
        attackFlowCoroutine = StartCoroutine(AttackFlow());
    }

    /// <summary>
    /// Stops the attack immediately in any of its phases.
    /// Can be safely called even if attack is not currently playing, as it will do nothing.
    /// </summary>
    public override void Stop()
    {
        if (Playing)
        {
            StopCoroutine(attackFlowCoroutine);
            InvokeOnStop();
        }

        if (Anticipating)
        {
            Anticipating = false;
            OnFinishAnticipating?.Invoke();
        }

        if (Active)
        {
            Active = false;
            OnFinishActive?.Invoke();
        }

        if (Recovering)
        {
            Recovering = false;
            OnFinishRecovery?.Invoke();
        }
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
        Recovering = false;
    }
}