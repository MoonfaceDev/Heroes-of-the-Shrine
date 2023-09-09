using System;

/// <summary>
/// Global component that registers events, with a similar API to <see cref="BaseComponent"/>.
/// Should be used only in classes that don't inherit from <see cref="BaseComponent"/>.
/// </summary>
public class GlobalEventManager : BaseComponent
{
    public static GlobalEventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Executes a callable every frame
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <returns>ID of the registered callable</returns>
    public string Register(Action action)
    {
        return eventManager.Register(action);
    }

    /// <summary>
    /// Unregisters a callable with given id. Nothing happens if <see cref="id"/> is not an existing callable
    /// </summary>
    /// <param name="id">ID of the callable to unregister</param>
    public void Unregister(string id)
    {
        eventManager.Unregister(id);
    }

    /// <summary>
    /// Executes a callable when a condition is met
    /// </summary>
    /// <param name="condition">Condition to check every frame</param>
    /// <param name="action">Callable to execute</param>
    /// <returns>ID of the registered callable</returns>
    public string InvokeWhen(Func<bool> condition, Action action)
    {
        return eventManager.InvokeWhen(condition, action);
    }

    /// <summary>
    /// Cancels a callable with given id. Nothing happens if <see cref="id"/> is not an existing callable
    /// </summary>
    /// <param name="id">ID of the callable to cancel</param>
    public void Cancel(string id)
    {
        eventManager.Cancel(id);
    }

    /// <summary>
    /// Executes a callable after a certain delay
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <param name="timeout">Time to wait before execution, in seconds</param>
    /// <returns>ID of the registered callable</returns>
    public string StartTimeout(Action action, float timeout)
    {
        return eventManager.StartTimeout(action, timeout);
    }

    /// <summary>
    /// Executes a callable repeatedly with a time delay between each call
    /// </summary>
    /// <param name="action">Callable to execute</param>
    /// <param name="interval">Time to wait between executions, in seconds</param>
    /// <returns>ID of the registered callable</returns>
    public string StartInterval(Action action, float interval)
    {
        return eventManager.StartInterval(action, interval);
    }
}