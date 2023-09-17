/// <summary>
/// Behaviour that heals the character
/// </summary>
public class HealBehaviour : PlayableBehaviour<HealBehaviour.Command>, IPlayableBehaviour
{
    public class Command
    {
    }

    public override bool Playing => Active;

    private bool Active { get; set; }

    public Cooldown cooldown;
    public float activeDuration;
    public float energyToHealthRatio;
    public float fullEnergyHealthReward;

    [InjectBehaviour] private HealthSystem healthSystem;
    [InjectBehaviour] private EnergySystem energySystem;
    private string stopTimeout;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && energySystem.Full
               && !Playing
               && cooldown.CanPlay();
    }

    protected override void DoPlay(Command command)
    {
        Active = true;
        energySystem.ResetEnergy();
        energySystem.onEnergyGrow += OnEnergyGrow;
        stopTimeout = eventManager.StartTimeout(Stop, activeDuration);
    }

    private void OnEnergyGrow(float energyAddition)
    {
        healthSystem.Heal(energyAddition * energyToHealthRatio);
        if (energySystem.Full)
        {
            healthSystem.Heal(fullEnergyHealthReward);
            Stop();
        }
    }

    protected override void DoStop()
    {
        eventManager.Cancel(stopTimeout);
        Active = false;
        cooldown.Reset();
        energySystem.onEnergyGrow -= OnEnergyGrow;
    }
}