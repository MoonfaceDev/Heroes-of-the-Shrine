using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CannotAttackException : Exception
{
}

[RequireComponent(typeof(AttackManager))]
public abstract class BaseAttack : PlayableBehaviour
{
    /// <summary>
    /// If true, the attack can be played while another interruptible attack is playing.
    /// </summary>
    public bool instant;
    
    /// <summary>
    /// If true, an instant attack can replace it while this attack is playing.
    /// </summary>
    [FormerlySerializedAs("interruptable")] public bool interruptible = true;
    
    /// <summary>
    /// If true, The attack cannot be interrupted while <c>Recovering</c> is true.
    /// </summary>
    public bool hardRecovery;

    /// <summary>
    /// Name of the attack, in the format <c>CamelCase</c>.
    /// Used in combo system, and in animator parameters.
    /// </summary>
    public string AttackName => GetType().Name;

    /// <summary>
    /// Attack is anticipating.
    /// It also sets the animator parameter: <c>{attackName}-anticipating</c>.
    /// </summary>
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

    /// <summary>
    /// Attack is active.
    /// It also sets the animator parameter: <c>{attackName}-active</c>.
    /// </summary>
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

    /// <summary>
    /// Attack is recovering.
    /// It also sets the animator parameter: <c>{attackName}-recovering</c>.
    /// </summary>
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

    /// <summary>
    /// Attack is playing.
    /// True if attack is either anticipating, active or recovering.
    /// </summary>
    public override bool Playing => Anticipating || Active || Recovering;

    /// <summary>
    /// Attack anticipation has started.
    /// </summary>
    public event Action OnStartAnticipating;
    
    /// <summary>
    /// Attack anticipation has finished.
    /// Also fires if the attack was stopped while in anticipation.
    /// </summary>
    public event Action OnFinishAnticipating;
    
    /// <summary>
    /// Attack active phase has started.
    /// </summary>
    public event Action OnStartActive;
    
    /// <summary>
    /// Attack active phase has finished.
    /// Also fires if the attack was stopped while in active phase.
    /// </summary>
    public event Action OnFinishActive;
    
    /// <summary>
    /// Attack recovery has started.
    /// </summary>
    public event Action OnStartRecovery;
    
    /// <summary>
    /// Attack recovery has finished.
    /// Also fires if the attack was stopped while in recovery.
    /// </summary>
    public event Action OnFinishRecovery;

    private bool anticipating;
    private bool active;
    private bool recovering;

    private Coroutine attackFlowCoroutine;

    /// <summary>
    /// Coroutine played while attack is anticipating.
    /// When the coroutine finishes, the anticipation finishes.
    /// </summary>
    protected abstract IEnumerator AnticipateCoroutine();
    
    /// <summary>
    /// Coroutine played while attack is active.
    /// When the coroutine finishes, the active phase finishes.
    /// </summary>
    protected abstract IEnumerator ActiveCoroutine();
    
    /// <summary>
    /// Coroutine played while attack is recovering.
    /// When the coroutine finishes, the recovery finishes.
    /// </summary>
    protected abstract IEnumerator RecoveryCoroutine();

    /// <summary>
    /// Tells if the attack can pe played.
    /// By default, any attack can be played if it is enabled, and the character is not under knockback or stunned.
    /// Override to add more conditions that attack requires.
    /// </summary>
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
