using System;
using Random = UnityEngine.Random;

/// <summary>
/// Hit executor that randomly applies electrify effect on hittables
/// </summary>
[Serializable]
public class ProbabilisticElectrifyHitExecutor : IHitExecutor
{
    /// <value>
    /// Executor to be executed if electrify effect was "decided" to be applied
    /// </value>
    public ElectrifyHitExecutor electrifyHitExecutor;
    
    /// <value>
    /// Probability that electrify effect is applied
    /// </value>
    public float probability;
    
    public void Execute(Hit hit)
    {
        if (!(Random.Range(0f, 1f) < probability)) return;
        hit.Victim.ProcessHit(electrifyHitExecutor, hit);
    }
}