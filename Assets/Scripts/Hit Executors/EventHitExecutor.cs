
using System;
using ExtEvents;
using UnityEngine;

[Serializable]
public class EventHitExecutor : IHitExecutor
{
    [SerializeField] public ExtEvent @event;
    
    public void Execute(Hit hit)
    {
        @event.Invoke();
    }
}