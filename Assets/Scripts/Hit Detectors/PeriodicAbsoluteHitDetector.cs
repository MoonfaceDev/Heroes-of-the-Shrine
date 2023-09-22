using System;
using ExtEvents.OdinSerializer.Utilities;

/// <summary>
/// An hit detector that detects hits periodically, with a given interval
/// </summary>
[Serializable]
public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    public SceneCollisionDetector collisionDetector;
    
    /// <value>
    /// Interval between detections, in seconds
    /// </value>
    public float interval;
    
    /// <value>
    /// If true, a detection occurs immediately when <see cref="BaseHitDetector.StartDetector"/> is called. Otherwise,
    /// the first detection occurs after a <see cref="interval"/>.
    /// </value>
    public bool startImmediately = true;

    private string detectionInterval;

    public override void StartDetector(Action<Collision> listener)
    {
        detectionInterval = GlobalEventManager.Instance.StartInterval(() => DetectHits(listener), interval);
        if (startImmediately)
        {
            DetectHits(listener);
        }
    }

    private void DetectHits(Action<Collision> listener)
    {
        collisionDetector.Detect().ForEach(listener);
    }

    public override void StopDetector()
    {
        GlobalEventManager.Instance.Unregister(detectionInterval);
    }
}