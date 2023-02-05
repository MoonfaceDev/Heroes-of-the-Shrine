using UnityEngine;

/// <summary>
/// State machine parameter that is set when health is below a certain value
/// </summary>
public class EnragedBrainModule : BrainModule
{
    /// <value>
    /// Parameter is set when health gets below this value
    /// </value>
    public float rageHealthThreshold;
    
    /// <value>
    /// Attacks damage multiplier when rage is on
    /// </value>
    public float rageDamageMultiplier = 1;

    private bool rage;

    private const string RageParameter = "rage";
    private static readonly int Rage = Animator.StringToHash(RageParameter);

    protected override void Awake()
    {
        base.Awake();

        InvokeWhen(ShouldGetEnraged, () =>
        {
            rage = true;
            StateMachine.SetBool(Rage, true);
        });
    }

    private void Start()
    {
        var attackManager = GetBehaviour<AttackManager>();
        if (attackManager)
        {
            attackManager.DamageTranspiler.AttachDamageMultiplier((_, _) => rage ? rageDamageMultiplier : 1);
        }
    }

    private bool ShouldGetEnraged()
    {
        var healthSystem = GetBehaviour<HealthSystem>();
        return healthSystem && healthSystem.Health <= rageHealthThreshold;
    }

    public override string[] GetParameters()
    {
        return new[] { RageParameter };
    }
}