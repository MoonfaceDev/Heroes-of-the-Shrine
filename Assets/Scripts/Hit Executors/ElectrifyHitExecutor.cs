using System;

/// <summary>
/// Hit executor that applies electrify effect to an hittable
/// </summary>
[Serializable]
public class ElectrifyHitExecutor : IHitExecutor
{
    /// <value>
    /// Effect duration
    /// </value>
    public float duration;
    
    /// <value>
    /// Speed reduction caused by the effect
    /// </value>
    public float speedMultiplier;
    
    public void Execute(Hit hit)
    {
        var electrifiedEffect = hit.victim.Character.GetBehaviour<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(new ElectrifiedEffect.Command {duration=duration, speedReductionMultiplier=speedMultiplier});
        }
    }
}