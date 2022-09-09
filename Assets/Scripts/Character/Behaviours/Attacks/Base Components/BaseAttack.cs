using System;
using System.Collections;
using UnityEngine;

public class CannotAttackException : Exception
{
}

[RequireComponent(typeof(AttackManager))]
public abstract class BaseAttack : CharacterBehaviour
{
    public string attackName;
    public bool instant = false;
    public bool interruptable = false;

    public delegate void OnAnticipate();
    public delegate void OnStart();
    public delegate void OnFinish();
    public delegate void OnRecover();

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

    public event OnAnticipate onAnticipate;
    public event OnStart onStart;
    public event OnFinish onFinish;
    public event OnRecover onRecover;

    protected void InvokeOnAnticipate()
    {
        onAnticipate?.Invoke();
    }

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

    protected abstract IEnumerator AnticipateCoroutine();
    protected abstract IEnumerator ActiveCoroutine();
    protected abstract IEnumerator RecoveryCoroutine();

    protected abstract bool CanAttack();
    protected abstract void HitCallable(HittableBehaviour hittableBehaviour);

    public void Attack()
    {
        if (!CanAttack())
        {
            throw new CannotAttackException();
        }
        StartCoroutine(AttackFlow());
    }

    private IEnumerator AttackFlow()
    {
        anticipating = true;
        InvokeOnAnticipate();
        yield return StartCoroutine(AnticipateCoroutine());

        anticipating = false;
        active = true;
        InvokeOnStart();
        yield return StartCoroutine(ActiveCoroutine());

        active = false;
        recovering = true;
        InvokeOnFinish();
        yield return StartCoroutine(RecoveryCoroutine());

        recovering = false;
        InvokeOnRecover();
    }
}
