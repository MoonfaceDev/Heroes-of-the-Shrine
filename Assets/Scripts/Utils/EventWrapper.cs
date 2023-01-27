using ExtEvents;
using UnityEngine;

public class EventWrapper : BaseComponent
{
    [SerializeField] public ExtEvent @event;

    public void Invoke()
    {
        @event.Invoke();
    }
}