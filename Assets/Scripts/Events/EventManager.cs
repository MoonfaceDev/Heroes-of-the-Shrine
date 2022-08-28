using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    private readonly HashSet<IEventListener> eventListeners = new();

    public void AttachEvent(IEventListener @event)
    {
        if (eventListeners.Contains(@event))
        {
            Debug.LogError("Event is already attached");
        }
        eventListeners.Add(@event);
    }

    public void DetachEvent(IEventListener @event)
    {
        eventListeners.Remove(@event);
    }

    public CallbackEvent Callback(CallbackCondition condition, CallbackAction action)
    {
        CallbackEvent callbackEvent = new(this, condition, action);
        AttachEvent(callbackEvent);
        return callbackEvent;
    }

    public void CancelCallback(CallbackEvent callbackEvent)
    {
        DetachEvent(callbackEvent);
    }

    // Update is called once per frame
    void Update()
    {
        IEventListener[] lockedEventListeners = new IEventListener[eventListeners.Count];
        eventListeners.CopyTo(lockedEventListeners);
        foreach (IEventListener eventListener in lockedEventListeners)
        {
            if (eventListener.Validate())
            {
                eventListener.Callback();
            }
        }
    }
}
