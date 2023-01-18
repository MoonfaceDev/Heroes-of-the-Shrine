using System;
using UnityEngine;

public class EnragedBrainModule : BrainModule
{
    public float rageHealthThreshold;
    public float rageDamageMultiplier = 1;

    private bool rage;

    private const string RageParameter = "rage";
    private static readonly int Rage = Animator.StringToHash(RageParameter);

    public override void Awake()
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
        var attackManager = GetComponent<AttackManager>();
        if (attackManager)
        {
            attackManager.DamageTranspiler.AttachDamageMultiplier((_, _) => rage ? rageDamageMultiplier : 1);
        }
    }

    private bool ShouldGetEnraged()
    {
        var healthSystem = GetComponent<HealthSystem>();
        return healthSystem && healthSystem.health <= rageHealthThreshold;
    }

    public override string[] GetParameters()
    {
        return new[] { RageParameter };
    }
}