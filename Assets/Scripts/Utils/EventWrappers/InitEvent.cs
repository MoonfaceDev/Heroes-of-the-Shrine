using ExtEvents;
using UnityEngine;

public class InitEvent : BaseComponent
{
    /// <value>
    /// Event to be invoked on init
    /// </value>
    [SerializeField] public ExtEvent @event;

    private void Start()
    {
        @event.Invoke();
    }
}