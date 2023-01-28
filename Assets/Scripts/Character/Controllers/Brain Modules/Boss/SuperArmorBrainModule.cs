using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="superArmorEffect"/>
/// </summary>
public class SuperArmorBrainModule : BrainModule
{
    private SuperArmorEffect superArmorEffect;

    private const string SuperArmorHealthParameter = "superArmorHealth";
    private static readonly int SuperArmorHealth = Animator.StringToHash(SuperArmorHealthParameter);

    protected override void Awake()
    {
        base.Awake();
        superArmorEffect = GetComponent<SuperArmorEffect>();
    }

    private void Start()
    {
        if (!superArmorEffect) return;
        UpdateSuperArmorHealth();
        superArmorEffect.onHit += UpdateSuperArmorHealth;
    }

    private void UpdateSuperArmorHealth()
    {
        StateMachine.SetFloat(SuperArmorHealth, superArmorEffect.armorHealth);
    }

    public override string[] GetParameters()
    {
        return new[] { SuperArmorHealthParameter };
    }
}