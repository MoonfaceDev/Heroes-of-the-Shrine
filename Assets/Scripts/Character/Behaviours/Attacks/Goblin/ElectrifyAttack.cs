using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")] public float electrifyDuration;
    public float electrifySpeedMultiplier;
    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public HitType periodicHitType = HitType.Stun;
    public float periodicKnockbackPower;
    public float periodicKnockbackDirection;
    public float periodicStunTime;
    public float periodicDamage;
    [Header("Explosion")] public BaseHitDetector explosionHitDetector;
    public UnityEvent onExplosion;

    [FormerlySerializedAs("epxlosionKnockbackPower")]
    public float explosionKnockbackPower;

    public float explosionKnockbackDirection;
    public float explosionDamage;
    public float explosionStunTime;

    protected override void ConfigureHitDetector()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        string switchDetectorsListener = null;

        generalEvents.onStartActive.AddListener(() =>
        {
            periodicHitDetector.StartDetector(HitCallable, AttackManager.hittableTags);
            switchDetectorsListener = InvokeWhen(() => detectCount >= periodicHitCount, () =>
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
            Cancel(switchDetectorsListener);
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
        if (periodicHitType == HitType.Stun)
        {
            hittable.Stun(processedDamage, periodicStunTime);
        }
        else
        {
            var hitDirection =
                (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x - MovableObject.WorldPosition.x);
            hittable.Knockback(processedDamage, periodicKnockbackPower,
                KnockbackBehaviour.GetRelativeDirection(periodicKnockbackDirection, hitDirection), periodicStunTime);
        }

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
        onExplosion.Invoke();
        var processedDamage = CalculateExplosionDamage(hittable.Character);
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