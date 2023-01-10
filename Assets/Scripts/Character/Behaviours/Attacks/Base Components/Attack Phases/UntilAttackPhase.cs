using System;
using System.Collections;
using UnityEngine;

public class UntilAttackPhase : IAttackPhase
{
    private readonly Func<bool> condition;

    public UntilAttackPhase(Func<bool> condition)
    {
        this.condition = condition;
    }

    public IEnumerator Play()
    {
        yield return new WaitUntil(condition);
    }
}