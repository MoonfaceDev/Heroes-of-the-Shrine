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
    public bool interruptable = false;

    public string attackName
    {
        get => GetType().Name;
    }

    public bool anticipating
    {
        get => _anticipating;
        private set
        {
            _anticipating = value;
            animator.SetBool(attackName + "-anticipating", _anticipating);
        }
    }

    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
            animator.SetBool(attackName + "-active", _active);
        }
    }

    public bool recovering
    {
        get => _recovering;
        private set
        {
            _recovering = value;
            animator.SetBool(attackName + "-recovering", _recovering);
        }
    }

    public override bool Playing => anticipating || active || recovering;

    public event Action onStart;
    public event Action onFinish;
    public event Action onRecover;

    protected void InvokeOnStart()
    {
        onStart?.Invoke();
    }

    protected void InvokeOnFinish()
    {
        onFinish?.Invoke();
    }

    protected void InvokeOnRecover()
    {
        onRecover?.Invoke();
    }

    private bool _anticipating;
    private bool _active;
    private bool _recovering;

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
        if (anticipating)
        {
            anticipating = false;
        }
        if (active)
        {
            active = false;
            InvokeOnFinish();
        }
        if (recovering)
        {
            recovering = false;
            InvokeOnRecover();
        }
    }

    private IEnumerator AttackFlow()
    {
        anticipating = true;
        InvokeOnPlay();
        yield return AnticipateCoroutine();

        anticipating = false;
        active = true;
        InvokeOnStart();
        yield return ActiveCoroutine();

        active = false;
        recovering = true;
        InvokeOnFinish();
        yield return RecoveryCoroutine();

        recovering = false;
        InvokeOnRecover();
        InvokeOnStop();
    }
}
