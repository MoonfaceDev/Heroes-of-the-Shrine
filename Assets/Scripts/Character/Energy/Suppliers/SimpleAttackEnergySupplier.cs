public class SimpleAttackEnergySupplier : BaseEnergySupplier
{
    public SimpleAttack attack;
    public float energyReward;

    // Every time attack is played, it can reward energy only once
    private bool alreadyAwarded;

    protected override void Awake()
    {
        base.Awake();

        attack.PlayEvents.onPlay += () => { alreadyAwarded = false; };

        attack.hitDetector.onHit += () =>
        {
            if (alreadyAwarded) return;
            alreadyAwarded = true;
            energySystem.energy += energyReward;
        };
    }
}