using UnityEngine;

public class HealthBrainModule : BrainModule
{
    private HealthSystem healthSystem;

    private const string HealthParameter = "health";
    private static readonly int Health = Animator.StringToHash(HealthParameter);

    public override void Awake()
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
    }

    public override string[] GetParameters()
    {
        return new[] { HealthParameter };
    }

    protected override void Update()
    {
        base.Update();
        if (!healthSystem) return;
        StateMachine.SetFloat(Health, healthSystem.health);
    }
}