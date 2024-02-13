using System;
using ExtEvents.OdinSerializer.Utilities;

/// <summary>
/// An hit detector that detects hits only in the frame it is started
/// </summary>
[Serializable]
public class AbsoluteHitDetector : BaseHitDetector
{
    public SceneCollisionDetector collisionDetector;

    public override void StartDetector(Action<Collision> listener)
    {
        collisionDetector.Detect().ForEach(listener);
    }

    public override void StopDetector()
    {
    }
}