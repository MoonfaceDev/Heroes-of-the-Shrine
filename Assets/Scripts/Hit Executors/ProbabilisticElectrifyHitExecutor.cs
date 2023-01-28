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
    
    public void Execute(BaseAttack attack, IHittable hittable)
    {
        if (!(Random.Range(0f, 1f) < probability)) return;
        electrifyHitExecutor.Execute(attack, hittable);
    }
}