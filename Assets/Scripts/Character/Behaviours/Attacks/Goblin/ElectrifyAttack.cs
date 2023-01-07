using UnityEngine;
using UnityEngine.Events;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")] public float electrifyDuration;
    public float electrifySpeedMultiplier;

    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public HitDefinition periodicHitDefinition;

    [Header("Explosion")] public BaseHitDetector explosionHitDetector;
    public HitDefinition explosionHitDefinition;
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
        var processedDamage = AttackManager.TranspileDamage(this, hittable, periodicHitDefinition.damage);
        if (periodicHitDefinition.hitType == HitType.Stun)
        {
            hittable.Stun(processedDamage, periodicHitDefinition.stunTime);
        }
        else
        {
            var hitDirection =
                (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x - MovableObject.WorldPosition.x);
            hittable.Knockback(processedDamage, periodicHitDefinition.knockbackPower,
                KnockbackBehaviour.GetRelativeDirection(periodicHitDefinition.knockbackDirection, hitDirection),
                periodicHitDefinition.stunTime);
        }

        if (Random.Range(0f, 1f) < periodicHitElectrifyRate)
        {
            var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                electrifiedEffect.Play(new ElectrifiedEffectCommand(electrifyDuration, electrifySpeedMultiplier));
            }
        }
    }

    private void ExplosionHitCallable(IHittable hittable)
    {
        onHit.Invoke(hittable);
        onExplosion.Invoke();
        var processedDamage = AttackManager.TranspileDamage(this, hittable, explosionHitDefinition.damage);
        var hitDirection =
            (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x - MovableObject.WorldPosition.x);
        hittable.Knockback(processedDamage, explosionHitDefinition.knockbackPower,
            KnockbackBehaviour.GetRelativeDirection(explosionHitDefinition.knockbackDirection, hitDirection),
            explosionHitDefinition.stunTime);
        var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(new ElectrifiedEffectCommand(electrifyDuration, electrifySpeedMultiplier));
        }
    }
}