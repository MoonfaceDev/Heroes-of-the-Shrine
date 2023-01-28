﻿using System;
using UnityEngine;

/// <summary>
/// Composite hit executor that executes all executors one by one 
/// </summary>
[Serializable]
public class ChainHitExecutor : IHitExecutor
{
    [SerializeInterface] [SerializeReference] public IHitExecutor[] executors;
    
    public void Execute(BaseAttack attack, IHittable hittable)
    {
        foreach (var executor in executors)
        {
            executor.Execute(attack, hittable);
        }
    }
}