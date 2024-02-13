using ExtEvents;
using UnityEngine;

/// <summary>
/// Invokes an event after waiting a given delay
/// </summary>
public class DelayedEvent : BaseComponent
{
    /// <value>
    /// Event to be invoked after the delay
    /// </value>
    [SerializeField] public ExtEvent @event;
    
    /// <value>
    /// Seconds to wait
    /// </value>
    public float delay;
    
    /// <summary>
    /// Invokes the event after waiting
    /// </summary>
    public void Invoke()
    {
        eventManager.StartTimeout(@event.Invoke, delay);
    }
}