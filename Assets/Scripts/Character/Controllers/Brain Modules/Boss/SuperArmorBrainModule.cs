using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="superArmor"/>
/// </summary>
public class SuperArmorBrainModule : BrainModule
{
    [InjectBehaviour] private SuperArmor superArmor;

    private const string SuperArmorHealthParameter = "superArmorHealth";
    private static readonly int SuperArmorHealth = Animator.StringToHash(SuperArmorHealthParameter);

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