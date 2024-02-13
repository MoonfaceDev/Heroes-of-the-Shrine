using System;

/// <summary>
/// Abstract base class for hit detectors, responsible for detecting hits and calling a given function for every hit object
/// </summary>
[Serializable]
public abstract class BaseHitDetector
{
    /// <summary>
    /// Starts detecting hits
    /// </summary>
    /// <param name="listener">Function to be called on detected hit</param>
    public abstract void StartDetector(Action<Collision> listener);

    /// <summary>
    /// Abstract method that stops detecting hits. Implementations should stop anything that <see cref="StartDetector"/> started.
    /// </summary>
    public abstract void StopDetector();
}