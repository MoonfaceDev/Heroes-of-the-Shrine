using ExtEvents;
using UnityEngine;

/// <summary>
/// Invoke an <see cref="ExtEvent"/>, which has more events than the normal <see cref="UnityEngine.Events.UnityEvent{T0}"/>
/// </summary>
public class EventWrapper : BaseComponent
{
    /// <value>
    /// Wrapped event
    /// </value>
    [SerializeField] public ExtEvent @event;

    /// <summary>
    /// Call the event
    /// </summary>
    public void Invoke()
    {
        @event.Invoke();
    }
}