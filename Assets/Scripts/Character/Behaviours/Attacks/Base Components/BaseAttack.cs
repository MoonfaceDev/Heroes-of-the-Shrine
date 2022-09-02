using UnityEngine;

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

    public abstract bool anticipating
    {
        get;
        protected set;
    }
    public abstract bool active
    {
        get;
        protected set;
    }
    public abstract bool recovering
    {
        get;
        protected set;
    }
    public event OnAnticipate onAnticipate;
    public event OnStart onStart;
    public event OnFinish onFinish;
    public event OnRecover onRecover;
    public abstract void Attack();
    public abstract bool CanAttack();
    public abstract bool ShouldTarget(HittableBehaviour hittableBehaviour);
    public abstract void OnHit(HittableBehaviour hittableBehaviour);
    
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
}
