using ExtEvents;
using UnityEngine;

/// <summary>
/// Behaviour that heals the character
/// </summary>
public class HealBehaviour : PlayableBehaviour<HealBehaviour.Command>, IControlledBehaviour
{
    public class Command
    {
    }

    public override bool Playing => Loading;

    public bool Loading
    {
        get => loading;
        private set
        {
            loading = value;
            Animator.SetBool(HealingParameter, value);
        }
    }

    /// <value>
    /// After heal is loaded for this long, it will stop loading 
    /// </value>
    public float maxLoadTime;

    /// <value>
    /// Energy decreased for a maximum healing effect. Energy is decreased in a constant rate. 
    /// </value>
    public float totalEnergyReduction;

    /// <value>
    /// Health increased for a maximum healing effect (not including <see cref="maxHealBonus"/>). Health is increased in a constant rate. 
    /// </value>
    public float totalHealthIncrement;

    /// <value>
    /// Health increased additionally if fully loaded
    /// </value>
    public float maxHealBonus;

    [SerializeField] public ExtEvent onMaxHeal;

    private bool loading;
    private float healStartTime;
    private string healListener;

    private static readonly int HealingParameter = Animator.StringToHash("healing");

    public override bool CanPlay(Command command)
    {
        var healthSystem = GetBehaviour<HealthSystem>();
        return base.CanPlay(command)
               && !IsPlaying<JumpBehaviour>()
               && healthSystem.Fraction < 1;
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(IControlledBehaviour));
        BlockBehaviours(typeof(IControlledBehaviour));

        Loading = true;

        var energySystem = GetBehaviour<EnergySystem>();
        var healthSystem = GetBehaviour<HealthSystem>();

        healStartTime = Time.time;

        healListener = Register(() =>
        {
            if (Time.time - healStartTime > maxLoadTime)
            {
                healthSystem.Health += maxHealBonus;
                onMaxHeal.Invoke();
                Stop();
                return;
            }
            energySystem.Energy -= Time.deltaTime * totalEnergyReduction / maxLoadTime;
            healthSystem.Health += Time.deltaTime * totalHealthIncrement / maxLoadTime;
            if (Mathf.Approximately(energySystem.Fraction, 0) || Mathf.Approximately(healthSystem.Fraction, 1))
            {
                Stop();
            }
        });
    }

    protected override void DoStop()
    {
        Loading = false;
        Unregister(healListener);
        UnblockBehaviours(typeof(IControlledBehaviour));
    }
}