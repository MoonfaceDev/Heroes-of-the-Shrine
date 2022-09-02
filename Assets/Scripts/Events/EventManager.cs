using System.Collections.Generic;
using UnityEngine;

public delegate bool EventCondition();
public delegate void EventAction();

public class EventListener
{
    public readonly EventCondition Validate;
    public readonly EventAction Action;
    public readonly bool single;

    public EventListener(EventCondition eventCondition, EventAction eventAction, bool single)
    {
        Validate = eventCondition;
        Action = eventAction;
        this.single = single;
    }
}

public class EventManager : MonoBehaviour
{

    private readonly HashSet<EventListener> eventListeners = new();

    public EventListener Attach(EventCondition condition, EventAction action, bool single = true)
    {
        EventListener eventListener = new(condition, action, single);
        eventListeners.Add(eventListener);
        return eventListener;
    }

    public void Detach(EventListener callbackEvent)
    {
        eventListeners.Remove(callbackEvent);
    }

    // Update is called once per frame
    void Update()
    {
        EventListener[] lockedEventListeners = new EventListener[eventListeners.Count];
        eventListeners.CopyTo(lockedEventListeners);
        foreach (EventListener eventListener in lockedEventListeners)
        {
            if (eventListener.Validate())
            {
                if (eventListener.single)
                {
                    Detach(eventListener);
                }
                eventListener.Action();
            }
        }
    }
}
