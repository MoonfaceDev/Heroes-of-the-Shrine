using UnityEngine;
using UnityEngine.Serialization;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")] public float electrifyDuration;
    public float electrifySpeedMultiplier;
    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public float periodicStunTime;
    public float periodicDamage;
    [Header("Explosion")] public BaseHitDetector explosionHitDetector;

    [FormerlySerializedAs("epxlosionKnockbackPower")]
    public float explosionKnockbackPower;

    public float explosionKnockbackDirection;
    public float explosionDamage;
    public float explosionStunTime;

    protected override void ConfigureHitDetector()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        EventListener switchDetectorsEvent = null;

        generalEvents.onStartActive.AddListener(() =>
        {
            periodicHitDetector.StartDetector(HitCallable, AttackManager.hittableTags);
            switchDetectorsEvent = EventManager.Attach(() => detectCount >= periodicHitCount, () =>
            {
                periodicHitDetector.StopDetector();
                explosionHitDetector.StartDetector(ExplosionHitCallable, AttackManager.hittableTags);
                detectCount = 0;
            });
        });

        generalEvents.onFinishActive.AddListener(() =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StopDetector();
            EventManager.Detach(switchDetectorsEvent);
        });
    }

    protected override float CalculateDamage(Character character)
    {
        return GetComponent<AttackManager>().TranspileDamage(this, character, periodicDamage);
    }

    protected override void HitCallable(IHittable hittable)
    {
        InvokeOnHit(hittable);
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