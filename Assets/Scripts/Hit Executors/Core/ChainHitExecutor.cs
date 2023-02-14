using System;
using UnityEngine;

/// <summary>
/// Composite hit executor that executes all executors one by one 
/// </summary>
[Serializable]
public class ChainHitExecutor : IHitExecutor
{
    /// <value>
    /// List of executors to call
    /// </value>
    [SerializeInterface] [SerializeReference] public IHitExecutor[] executors;
    
    public void Execute(BaseAttack attack, IHittable hittable)
    {
        if (TryBlock(attack, hittable))
        {
            return;
        }
        foreach (var executor in executors)
        {
            executor.Execute(attack, hittable);
        }
    }

    private bool TryBlock(BaseAttack attack, IHittable hittable)
    {
        var focusBlock = hittable.Character.GetBehaviour<FocusBlock>();
        return focusBlock && focusBlock.TryBlock(attack);
    }
}