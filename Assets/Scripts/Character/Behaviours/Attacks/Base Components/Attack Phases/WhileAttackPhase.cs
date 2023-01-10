using System;
using System.Collections;
using UnityEngine;

public class WhileAttackPhase : IAttackPhase
{
    private readonly Func<bool> condition;

    public WhileAttackPhase(Func<bool> condition)
    {
        this.condition = condition;
    }

    public IEnumerator Play()
    {
        yield return new WaitWhile(condition);
    }
}