using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for every component, extending <see cref="MonoBehaviour"/> with more features
/// </summary>
public class BaseComponent : MonoBehaviour
{
    protected readonly EventManager eventManager = new();

    protected virtual void Update()
    {
        eventManager.Tick();
    }
}