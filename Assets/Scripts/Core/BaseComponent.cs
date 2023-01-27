using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseComponent : MonoBehaviour
{
    private struct EventListener
    {
        public readonly Action action;
        public readonly Func<bool> condition;

        public EventListener(Action action, Func<bool> condition = null)
        {
            this.action = action;
            this.condition = condition;
        }
    }

    private readonly Dictionary<string, EventListener> eventListeners = new();

    /// <summary>
    /// Executes a callable every frame
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <returns>ID of the registered callable</returns>
    protected string Register(Action action)
    {
        var id = Guid.NewGuid().ToString();
        eventListeners.Add(id, new EventListener(action));
        return id;
    }

    /// <summary>
    /// Unregisters a callable with given id. Nothing happens if <see cref="id"/> is not an existing callable
    /// </summary>
    /// <param name="id">ID of the callable to unregister</param>
    protected void Unregister(string id)
    {
        if (id != null)
        {
            eventListeners.Remove(id);
        }
    }

    /// <summary>
    /// Executes a callable when a condition is met
    /// </summary>
    /// <param name="condition">Condition to check every frame</param>
    /// <param name="action">Callable to execute</param>
    /// <returns>ID of the registered callable</returns>
    protected string InvokeWhen(Func<bool> condition, Action action)
    {
        var id = Guid.NewGuid().ToString();
        eventListeners.Add(id, new EventListener(action, condition));
        return id;
    }

    /// <summary>
    /// Cancels a callable with given id. Nothing happens if <see cref="id"/> is not an existing callable
    /// </summary>
    /// <param name="id">ID of the callable to cancel</param>
    protected void Cancel(string id)
    {
        Unregister(id);
    }

    /// <summary>
    /// Executes a callable after a certain delay
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <param name="timeout">Time to wait before execution, in seconds</param>
    /// <returns>ID of the registered callable</returns>
    protected string StartTimeout(Action action, float timeout)
    {
        var startTime = Time.time;
        return InvokeWhen(() => Time.time - startTime >= timeout, action);
    }

    /// <summary>
    /// Executes a callable repeatedly with a time delay between each call
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <param name="interval">Time to wait between executions, in seconds</param>
    /// <returns>ID of the registered callable</returns>
    protected string StartInterval(Action action, float interval)
    {
        var startTime = Time.time;
        var lastRunTime = startTime;
        return Register(() =>
        {
            if (Time.time - lastRunTime > interval)
            {
                lastRunTime = Time.time;
                action();
            }
        });
    }
    
    protected virtual void Update()
    {
        var lockedEventListeners = new Dictionary<string, EventListener>(eventListeners);
        foreach (var id in lockedEventListeners.Keys)
        {
            ExecuteEvent(id);
        }
    }

    private void ExecuteEvent(string id)
    {
        if (!eventListeners.ContainsKey(id)) return; // Event was already detached
        var eventListener = eventListeners[id];
        if (eventListener.condition == null)
        {
            eventListener.action();
        }
        else if (eventListener.condition())
        {
            eventListener.action();
            Cancel(id);
        }
    }
}