using UnityEngine;
using UnityEngine.Serialization;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")]
    public float electrifyDuration;
    public float electrifySpeedMultiplier;
    [Header("Periodic hits")]
    public Hitbox periodicHitbox;
    public float periodicHitInterval;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public float periodicStunTime;
    public float periodicDamage;
    [Header("Explosion")]
    public Hitbox explosionHitbox;
    [FormerlySerializedAs("epxlosionKnockbackPower")] public float explosionKnockbackPower;
    public float explosionKnockbackDirection;
    public float explosionDamage;

    private PeriodicAbsoluteHitDetector periodicHitDetector;
    private SingleHitDetector explosionHitDetector;

    protected override void CreateHitDetector()
    {
        periodicHitDetector = new PeriodicAbsoluteHitDetector(EventManager, periodicHitbox, hittable =>
        {
            if (!IsHittableTag(hittable.tag)) return;
            if (hittable.CanGetHit())
            {
                periodicHitbox.PlayParticles();
            }
            HitCallable(hittable);
        }, periodicHitInterval);

        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        explosionHitDetector = new SingleHitDetector(EventManager, explosionHitbox, hittable =>
        {
            if (!IsHittableTag(hittable.tag)) return;
            if (hittable.CanGetHit())
            {
                explosionHitbox.PlayParticles();
            }
            ExplosionHitCallable(hittable);
        });

        EventListener switchDetectorsEvent = null;

        OnStartActive += () =>
        {
            periodicHitDetector.Start();
            switchDetectorsEvent = EventManager.Attach(() => detectCount >= periodicHitCount, () =>
            {
                periodicHitDetector.Stop();
                explosionHitDetector.Start();
                detectCount = 0;
            });
        };

        OnFinishActive += () =>
        {
            periodicHitDetector.Stop();
            explosionHitDetector.Stop();
            EventManager.Detach(switchDetectorsEvent);
        };
    }

    protected override float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, hittableBehaviour, periodicDamage);
    }

    protected override void HitCallable(HittableBehaviour hittableBehaviour)
    {
        var processedDamage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by periodic " + AttackName);
        hittableBehaviour.Stun(processedDamage, periodicStunTime);
        if (Random.Range(0f, 1f) < periodicHitElectrifyRate)
        {
            var electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
            }
        }
    }

    private float CalculateExplosionDamage(HittableBehaviour hittableBehaviour)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, hittableBehaviour, explosionDamage);
    }

    private void ExplosionHitCallable(HittableBehaviour hittableBehaviour)
    {
        var processedDamage = CalculateExplosionDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by explosion " + AttackName);
        var hitDirection = (int)Mathf.Sign(hittableBehaviour.MovableObject.WorldPosition.x - MovableObject.WorldPosition.x);
        hittableBehaviour.Knockback(processedDamage, explosionKnockbackPower, KnockbackBehaviour.GetRelativeDirection(explosionKnockbackDirection, hitDirection));
        var electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
        }
    }
}
