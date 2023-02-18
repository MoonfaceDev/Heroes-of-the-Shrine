using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="superArmor"/>
/// </summary>
public class SuperArmorBrainModule : BrainModule
{
    private SuperArmor superArmor;

    private const string SuperArmorHealthParameter = "superArmorHealth";
    private static readonly int SuperArmorHealth = Animator.StringToHash(SuperArmorHealthParameter);

    protected override void Awake()
    {
        base.Awake();
        superArmor = GetBehaviour<SuperArmor>();
    }

    private void Start()
    {
        UpdateSuperArmorHealth();
        superArmor.PlayEvents.onPlay += UpdateSuperArmorHealth;
        superArmor.onHit += UpdateSuperArmorHealth;
    }

    private void UpdateSuperArmorHealth()
    {
        StateMachine.SetFloat(SuperArmorHealth, superArmor.CurrentArmorHealth);
    }

    public override string[] GetParameters()
    {
        return new[] { SuperArmorHealthParameter };
    }
}