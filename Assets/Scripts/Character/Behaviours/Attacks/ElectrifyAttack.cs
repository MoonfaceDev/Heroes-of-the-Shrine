using UnityEngine;
using UnityEngine.Serialization;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")] public float electrifyDuration;
    public float electrifySpeedMultiplier;
    [Header("Periodic hits")] public Hitbox periodicHitbox;
    public float periodicHitInterval;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public float periodicStunTime;
    public float periodicDamage;
    [Header("Explosion")] public Hitbox explosionHitbox;

    [FormerlySerializedAs("epxlosionKnockbackPower")]
    public float explosionKnockbackPower;

    public float explosionKnockbackDirection;
    public float explosionDamage;
    public float explosionStunTime;

    private PeriodicAbsoluteHitDetector periodicHitDetector;
    private SingleHitDetector explosionHitDetector;

    protected override void CreateHitDetector()
    {
        periodicHitDetector = new PeriodicAbsoluteHitDetector(EventManager, periodicHitbox, hittable =>
        {
            if (!IsHittableTag(hittable.Character.tag)) return;
            periodicHitbox.PlayParticles();
            HitCallable(hittable);
        }, periodicHitInterval);

        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        explosionHitDetector = new SingleHitDetector(EventManager, explosionHitbox, hittable =>
        {
            if (!IsHittableTag(hittable.Character.tag)) return;
            explosionHitbox.PlayParticles();
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

    protected override float CalculateDamage(Character character)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, character, periodicDamage);
    }

    protected override void HitCallable(IHittable hittable)
    {
        var processedDamage = CalculateDamage(hittable.Character);
        print(hittable.Character.name + " hit by periodic " + AttackName);
        hittable.Stun(processedDamage, periodicStunTime);
        if (Random.Range(0f, 1f) < periodicHitElectrifyRate)
        {
            var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
            }
        }
    }

    private float CalculateExplosionDamage(Character character)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, character, explosionDamage);
    }

    private void ExplosionHitCallable(IHittable hittable)
    {
        var processedDamage = CalculateExplosionDamage(hittable.Character);
        print(hittable.Character.name + " hit by explosion " + AttackName);
        var hitDirection =
            (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x - MovableObject.WorldPosition.x);
        hittable.Knockback(processedDamage, explosionKnockbackPower,
            KnockbackBehaviour.GetRelativeDirection(explosionKnockbackDirection, hitDirection), explosionStunTime);
        var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(electrifyDuration, electrifySpeedMultiplier);
        }
    }
}