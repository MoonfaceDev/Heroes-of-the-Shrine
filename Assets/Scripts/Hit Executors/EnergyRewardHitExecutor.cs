using System;

[Serializable]
public class EnergyRewardHitExecutor : IHitExecutor
{
    public float energyReward;

    public void Execute(Hit hit)
    {
        hit.Source.GetBehaviour<EnergySystem>().AddEnergy(energyReward);
    }
}