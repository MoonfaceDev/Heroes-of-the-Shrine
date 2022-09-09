using System.Collections;
using UnityEngine;

public class EnergyBurstAttack : SimpleAttack
{
    public Hitbox hitbox;
    public float burstVelocity;
    public float burstAcceleration;

    private SingleHitDetector hitDetector;

    public override void Awake()
    {
        base.Awake();
        hitDetector = new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (hittableBehaviour)
            {
                HitCallable(hittableBehaviour);
            }
        });
        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop(true);
        };
    }

    protected override IEnumerator ActiveCoroutine()
    {
        bool stopped = false;
        float direction = Mathf.Sign(movableObject.velocity.x);
        movableObject.velocity.x = direction * burstVelocity;
        movableObject.acceleration.x = -direction * burstAcceleration;
        eventManager.Attach(() => Mathf.Sign(movableObject.velocity.x) != direction, () => stopped = true);
        hitDetector.Start();
        yield return new WaitUntil(() => stopped);
        hitDetector.Stop();
    }

    protected override float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return damage;
    }
}
