using System.Collections;
using UnityEngine;

public class ElectrifyAttack : NormalAttack
{
    [Header("Electrify")]
    public float electrifyDuration;
    public float electrifySpeedMultiplier;
    [Header("Periodic hits")]
    public float periodicHitInterval;
    public int periodicHitCount;
    public float periodicHitElectrifyRate;
    public float periodicStunTime;
    public Hitbox periodicHitbox;
    [Header("Explosion")]
    public Hitbox explosionHitbox;

    private PeriodicAbsoluteHitDetector periodicHitDetector;
    private SingleHitDetector explosionHitDetector;

    protected override void CreateHitDetector()
    {
        periodicHitDetector = new(eventManager, periodicHitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
            {
                HitCallable(hittable);
            }
        }, periodicHitInterval);
        
        explosionHitDetector = new(eventManager, explosionHitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
            {
                ExplosionHitCallable(hittable);
            }
        });

        Coroutine switchHitDetectors = null;

        onStart += () =>
        {
            periodicHitDetector.Start();
            switchHitDetectors = StartCoroutine(SwitchHitDetectors());
        };

        onFinish += () =>
        {
            periodicHitDetector.Stop();
            explosionHitDetector.Stop();
        };

        onStop += () =>
        {
            StopCoroutine(switchHitDetectors);
        };
    }

    protected override void HitCallable(HittableBehaviour hittableBehaviour)
    {
        float damage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by " + attackName);
        hittableBehaviour.Stun(damage, stunTime);
        if (Random.Range(0f, 1f) < periodicHitElectrifyRate)
        {
            ElectrifiedEffect electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
            if (electrifiedEffect)
            {
                electrifiedEffect.Activate(electrifyDuration, electrifySpeedMultiplier);
            }
            else
            {
                Debug.LogWarning(hittableBehaviour.name + " doesn't have an ElectrifiedEffect component");
            }
        }
    }

    protected void ExplosionHitCallable(HittableBehaviour hittableBehaviour)
    {
        float damage = CalculateDamage(hittableBehaviour);
        print(hittableBehaviour.name + " hit by " + attackName);
        int hitDirection = (int)Mathf.Sign(hittableBehaviour.movableObject.position.x - movableObject.position.x);
        hittableBehaviour.Knockback(damage, knockbackPower, KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection));
        ElectrifiedEffect electrifiedEffect = hittableBehaviour.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Activate(electrifyDuration, electrifySpeedMultiplier);
        }
        else
        {
            Debug.LogWarning(hittableBehaviour.name + " doesn't have an ElectrifiedEffect component");
        }
    }

    private IEnumerator SwitchHitDetectors()
    {
        yield return new WaitForSeconds(periodicHitInterval * (periodicHitCount + 1));
        periodicHitDetector.Stop();
        explosionHitDetector.Start();
    }
}
