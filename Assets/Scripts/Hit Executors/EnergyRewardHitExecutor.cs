using System;

[Serializable]
public class EnergyRewardHitExecutor : IHitExecutor
{
    public float energyReward;

    public void Execute(Hit hit)
    {
        hit.source.GetBehaviour<EnergySystem>().Energy += energyReward;
    }
}