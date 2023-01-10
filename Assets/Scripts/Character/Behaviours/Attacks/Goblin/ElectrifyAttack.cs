using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
public class ElectrifyEffectDefinition
{
    public float duration;
    public float speedMultiplier;
}

[Serializable]
public class PeriodicHitDefinition : HitDefinition
{
    public float electrifyRate;
    public ElectrifyEffectDefinition electrifyEffectDefinition;
}

[Serializable]
public class ExplosionHitDefinition : HitDefinition
{
    public ElectrifyEffectDefinition electrifyEffectDefinition;
}

public class PeriodicHitExecutor : HitExecutor<PeriodicHitDefinition>
{
    public PeriodicHitExecutor(PeriodicHitDefinition hitDefinition)
    {
        HitDefinition = hitDefinition;
    }

    protected override PeriodicHitDefinition HitDefinition { get; }

    public override void Execute(BaseAttack attack, IHittable hittable)
    {
        base.Execute(attack, hittable);
        if (Random.Range(0f, 1f) < HitDefinition.electrifyRate)
        {
            var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                var electrifyEffectDefinition = HitDefinition.electrifyEffectDefinition;
                electrifiedEffect.Play(new ElectrifiedEffectCommand(electrifyEffectDefinition.duration,
                    electrifyEffectDefinition.speedMultiplier));
            }
        }
    }
}

public class ExplosionHitExecutor : HitExecutor<ExplosionHitDefinition>
{
    public ExplosionHitExecutor(ExplosionHitDefinition hitDefinition)
    {
        HitDefinition = hitDefinition;
    }

    protected override ExplosionHitDefinition HitDefinition { get; }

    public override void Execute(BaseAttack attack, IHittable hittable)
    {
        base.Execute(attack, hittable);
        var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            var electrifyEffectDefinition = HitDefinition.electrifyEffectDefinition;
            electrifiedEffect.Play(new ElectrifiedEffectCommand(electrifyEffectDefinition.duration,
                electrifyEffectDefinition.speedMultiplier));
        }
    }
}

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")] public float electrifyDuration; // TODO: Remove
    public float electrifySpeedMultiplier; // TODO: Remove

    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public float periodicHitElectrifyRate; // TODO: Remove
    public PeriodicHitDefinition periodicHitDefinition;

    [Header("Explosion")] public BaseHitDetector explosionHitDetector;
    public ExplosionHitDefinition explosionHitDefinition;
    public UnityEvent onExplosion;

    protected override void ConfigureHitDetector()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        string switchDetectorsListener = null;

        attackEvents.onStartActive.AddListener(() =>
        {
            periodicHitDetector.StartDetector(HitCallable, AttackManager.hittableTags);
            switchDetectorsListener = InvokeWhen(() => detectCount >= periodicHitCount, () =>
            {
                periodicHitDetector.StopDetector();
                explosionHitDetector.StartDetector(ExplosionHitCallable, AttackManager.hittableTags);
                detectCount = 0;
            });
        });

        attackEvents.onFinishActive.AddListener(() =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StopDetector();
            Cancel(switchDetectorsListener);
        });
    }

    protected override void HitCallable(IHittable hittable)
    {
        onHit.Invoke(hittable);
        new PeriodicHitExecutor(periodicHitDefinition).Execute(this, hittable);
    }

    private void ExplosionHitCallable(IHittable hittable)
    {
        onHit.Invoke(hittable);
        onExplosion.Invoke();
        new ExplosionHitExecutor(explosionHitDefinition).Execute(this, hittable);
    }
}