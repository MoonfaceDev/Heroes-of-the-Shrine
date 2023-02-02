using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base class for every component, extending <see cref="MonoBehaviour"/> with more features
/// </summary>
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
    /// If <c>true</c>, the component is updated (<see cref="Update"/> is called).
    /// Can be set to <c>false</c> multiple times, as it will change back to <c>true</c> only if it was set to <c>true</c> the same number of times.
    /// </summary>
    public bool Enabled
    {
        get => disableCount == 0;
        set
        {
            if (value)
            {
                if (disableCount > 0)
                {
                    disableCount--;
                }
            }
            else
            {
                disableCount++;
            }

            enabled = disableCount == 0;
        }
    }

    private int disableCount;

    private void SetEnabled(bool enabledValue, IEnumerable<Type> components)
    {
        foreach (var type in components)
        {
            foreach (var component in GetComponents(type).Cast<BaseComponent>())
            {
                component.Enabled = enabledValue;
            }
        }
    }

    /// <summary>
    /// Enables components, meaning they are updated
    /// </summary>
    /// <param name="components">Components to enable. All of the attached components from each type will be enabled.</param>
    public void EnableComponents(params Type[] components)
    {
        SetEnabled(true, components);
    }

    /// <summary>
    /// Disables components, meaning they are not updated. If a component is disabled N times, it will have to be enabled N times so it enabled again.
    /// </summary>
    /// <param name="components">Components to disable. All of the attached components from each type will be enabled.</param>
    public void DisableComponents(params Type[] components)
    {
        SetEnabled(false, components);
    }

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