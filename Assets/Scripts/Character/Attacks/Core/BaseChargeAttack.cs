using System.Collections;
using UnityEngine;

public abstract class BaseChargeAttack : BaseAttack
{
    private bool released;

    protected override void Awake()
    {
        base.Awake();
        phaseEvents.onFinishActive += () => released = false;
    }
    
    protected override IEnumerator ActivePhase()
    {
        yield return new WaitUntil(() => (released && CanRelease()) || MustRelease());
    }

    protected abstract bool CanRelease();

    protected abstract bool MustRelease();

    public void Release()
    {
        if (Active)
        {
            released = true;
        }
    }
}