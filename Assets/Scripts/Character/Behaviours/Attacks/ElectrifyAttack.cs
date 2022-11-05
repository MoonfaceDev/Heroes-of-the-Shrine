using UnityEngine;

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
    public float epxlosionKnockbackPower;
    public float explosionKnockbackDirection;
    public float explosionDamage;

    private PeriodicAbsoluteHitDetector periodicHitDetector;
    private SingleHitDetector explosionHitDetector;

    protected override void CreateHitDetector()
    {
        periodicHitDetector = new(EventManager, periodicHitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
            {
                HitCallable(hittable);
            }
        }, periodicHitInterval);

        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        explosionHitDetector = new(EventManager, explosionHitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
            {
                ExplosionHitCallable(hittable);
            }
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
        float damage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by periodic " + AttackName);
        if (hittableBehaviour.CanGetHit())
        {
            periodicHitbox.PlayParticles();
        }
        hittableBehaviour.Stun(damage, periodicStunTime);
        if (Random.Range(0f, 1f) < periodicHitElectrifyRate)
        {
            ElectrifiedEffect electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
            }
        }
    }
    protected float CalculateExplosionDamage(HittableBehaviour hittableBehaviour)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, hittableBehaviour, explosionDamage);
    }

    protected void ExplosionHitCallable(HittableBehaviour hittableBehaviour)
    {
        float damage = CalculateExplosionDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by explosion " + AttackName);
        if (hittableBehaviour.CanGetHit())
        {
            explosionHitbox.PlayParticles();
        }
        int hitDirection = (int)Mathf.Sign(hittableBehaviour.MovableObject.position.x - MovableObject.position.x);
        hittableBehaviour.Knockback(damage, epxlosionKnockbackPower, KnockbackBehaviour.GetRelativeDirection(explosionKnockbackDirection, hitDirection));
        ElectrifiedEffect electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
        }
    }
}
