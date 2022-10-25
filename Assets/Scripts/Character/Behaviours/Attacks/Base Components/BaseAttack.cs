using System;
using System.Collections;
using UnityEngine;

public class CannotAttackException : Exception
{
}

[RequireComponent(typeof(AttackManager))]
public abstract class BaseAttack : PlayableBehaviour
{
    public bool instant = false;
    public bool interruptable = true;
    public bool hardRecovery = false;

    public string AttackName
    {
        get => GetType().Name;
    }

    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(AttackName + "-anticipating", anticipating);
        }
    }

    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(AttackName + "-active", active);
        }
    }

    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(AttackName + "-recovering", recovering);
        }
    }

    public override bool Playing => Anticipating || Active || Recovering;

    public event Action OnStart;
    public event Action OnFinish;
    public event Action OnRecover;

    protected void InvokeOnStart()
    {
        OnStart?.Invoke();
    }

    protected void InvokeOnFinish()
    {
        OnFinish?.Invoke();
    }

    protected void InvokeOnRecover()
    {
        OnRecover?.Invoke();
    }

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
        attackFlowCoroutine = StartCoroutine(AttackFlow());
    }

    public override void Stop()
    {
        if (Playing)
        {
            InvokeOnStop();
            StopCoroutine(attackFlowCoroutine);
        }
        if (Anticipating)
        {
            Anticipating = false;
        }
        if (Active)
        {
            Active = false;
            InvokeOnFinish();
        }
        if (Recovering)
        {
            Recovering = false;
            InvokeOnRecover();
        }
    }

    private IEnumerator AttackFlow()
    {
        Anticipating = true;
        InvokeOnPlay();
        yield return AnticipateCoroutine();

        Anticipating = false;
        Active = true;
        InvokeOnStart();
        yield return ActiveCoroutine();

        Active = false;
        Recovering = true;
        InvokeOnFinish();
        yield return RecoveryCoroutine();

        Recovering = false;
        InvokeOnRecover();
        InvokeOnStop();
    }
}
