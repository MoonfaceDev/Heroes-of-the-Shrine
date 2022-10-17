using System;
using System.Collections.Generic;
using UnityEngine;

public class EventListener
{
    public readonly Func<bool> Condition;
    public readonly Action Action;
    public readonly bool single;

    public EventListener(Func<bool> eventCondition, Action eventAction, bool single)
    {
        Condition = eventCondition;
        Action = eventAction;
        this.single = single;
    }
}

public class EventManager : MonoBehaviour
{

    private readonly HashSet<EventListener> eventListeners = new();

    public EventListener Attach(Func<bool> condition, Action action, bool single = true)
    {
        EventListener eventListener = new(condition, action, single);
        eventListeners.Add(eventListener);
        return eventListener;
    }

    public void Detach(EventListener callbackEvent)
    {
        eventListeners.Remove(callbackEvent);
    }

    public EventListener StartTimeout(Action action, float timeout)
    {
        float startTime = Time.time;
        return Attach(() => Time.time - startTime >= timeout, action);
    }

    public EventListener StartInterval(Action action, float interval)
    {
        float startTime = 0;
        void StartOneInterval()
        {
            startTime = Time.time;
        }
        StartOneInterval();
        return Attach(() => Time.time - startTime >= interval, StartOneInterval + action, false);
    }

    // Update is called once per frame
    void Update()
    {
        EventListener[] lockedEventListeners = new EventListener[eventListeners.Count];
        eventListeners.CopyTo(lockedEventListeners);
        foreach (EventListener eventListener in lockedEventListeners)
        {
            if (eventListener.Condition())
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
