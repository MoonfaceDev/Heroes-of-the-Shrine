public class SpinningSwordsAttackEnergySupplier : BaseEnergySupplier
{
    public SpinningSwordsAttack attack;
    public float energyReward;

    // Every time attack is played, it can reward energy only once
    private bool alreadyAwarded1;
    private bool alreadyAwarded2;

    protected override void Awake()
    {
        base.Awake();

        attack.PlayEvents.onPlay += () =>
        {
            alreadyAwarded1 = false;
            alreadyAwarded2 = false;
        };

        attack.hitDetector1.onHit += () =>
        {
            if (alreadyAwarded1) return;
            alreadyAwarded1 = true;
            energySystem.energy += energyReward;
        };
        
        attack.hitDetector2.onHit += () =>
        {
            if (alreadyAwarded2) return;
            alreadyAwarded2 = true;
            energySystem.energy += energyReward;
        };
    }
}