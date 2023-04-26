using UnityEngine;

/// <summary>
/// Behaviour that heals the character
/// </summary>
public class HealBehaviour : PlayableBehaviour<HealBehaviour.Command>, IControlledBehaviour
{
    public class Command
    {
    }

    public override bool Playing => Active;

    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(HealingParameter, value);
        }
    }

    public Cooldown cooldown;
    public float energyToHealthRatio;
    public float fullEnergyHealthReward;

    private bool active;
    private HealthSystem healthSystem;
    private EnergySystem energySystem;

    private static readonly int HealingParameter = Animator.StringToHash("healing");

    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetBehaviour<HealthSystem>();
        energySystem = GetBehaviour<EnergySystem>();
        GetBehaviour<HittableBehaviour>().onHit += Stop;
    }

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && energySystem.Full
               && cooldown.CanPlay();
    }

    protected override void DoPlay(Command command)
    {
        cooldown.Reset();
        Active = true;
        energySystem.onEnergyGrow += OnEnergyGrow;
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
        Active = false;
        energySystem.onEnergyGrow -= OnEnergyGrow;
    }
}