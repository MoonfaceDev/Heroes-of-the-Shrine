using ExtEvents;
using UnityEngine;

public class DelayedEvent : BaseComponent
{
    [SerializeField] public ExtEvent @event;
    public float delay;
    
    public void Invoke()
    {
        StartTimeout(@event.Invoke, delay);
    }
}