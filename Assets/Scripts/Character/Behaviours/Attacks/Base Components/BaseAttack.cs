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
    public bool instant;
    [FormerlySerializedAs("interruptable")] public bool interruptible = true;
    public bool hardRecovery;

    public string AttackName => GetType().Name;

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

    public override bool Playing => Anticipating || Active || Recovering;

    public event Action OnStartAnticipating;
    public event Action OnFinishAnticipating;
    public event Action OnStartActive;
    public event Action OnFinishActive;
    public event Action OnStartRecovery;
    public event Action OnFinishRecovery;

    private bool anticipating;
    private bool active;
    private bool recovering;

    private Coroutine attackFlowCoroutine;

    protected abstract IEnumerator AnticipateCoroutine();
    protected abstract IEnumerator ActiveCoroutine();
    protected abstract IEnumerator RecoveryCoroutine();

    public override bool CanPlay()
    {
        return base.CanPlay() && AllStopped(typeof(KnockbackBehaviour), typeof(StunBehaviour));
    }
    protected abstract void HitCallable(HittableBehaviour hittableBehaviour);

    public void Play()
    {
        if (!CanPlay())
        {
            throw new CannotAttackException();
        }
        InvokeOnPlay();
        attackFlowCoroutine = StartCoroutine(AttackFlow());
    }

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
