using System;
using UnityEngine;

/// <summary>
/// Composite hit executor that executes all executors one by one 
/// </summary>
[Serializable]
public class ChainHitExecutor
{
    /// <value>
    /// List of executors to call
    /// </value>
    [SerializeInterface] [SerializeReference] public IHitExecutor[] executors;
    
    public void Execute(Hit hit)
    {
        hit.victim.HitEvent.Invoke();
        foreach (var executor in executors)
        {
            hit.victim.Hit(executor, hit);
        }
    }
}