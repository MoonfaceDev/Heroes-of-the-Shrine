using System;
using System.Collections.Generic;
using UnityEngine;

public class EventListener
{
    public readonly Func<bool> condition;
    public readonly Action action;
    public readonly bool single;

    public EventListener(Func<bool> eventCondition, Action eventAction, bool single)
    {
        condition = eventCondition;
        action = eventAction;
        this.single = single;
    }
}

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

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
        var startTime = Time.time;
        return Attach(() => Time.time - startTime >= timeout, action);
    }

    public delegate void StopInterval();

    public EventListener StartInterval(Action<StopInterval> action, float interval)
    {
        float startTime;
        void StartOneInterval()
        {
            startTime = Time.time;
        }
        StartOneInterval();
        EventListener eventListener = null;
        eventListener = Attach(() => Time.time - startTime >= interval, () => { 
            StartOneInterval();
            action(() => Detach(eventListener));
            }, false);
        return eventListener;
    }

    public EventListener StartInterval(Action action, float interval)
    {
        float startTime;
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
        var lockedEventListeners = new EventListener[eventListeners.Count];
        eventListeners.CopyTo(lockedEventListeners);
        foreach (var eventListener in lockedEventListeners)
        {
            if (!eventListener.condition()) continue;
            if (eventListener.single)
            {
                Detach(eventListener);
            }
            eventListener.action();
        }
    }
}