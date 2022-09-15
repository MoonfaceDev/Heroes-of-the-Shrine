using System.Collections;
using UnityEngine;

public class EnergyBurstAttack : SimpleAttack
{
    public Hitbox hitbox;
    public float burstVelocity;
    public float burstAcceleration;

    private bool isMoving = false;

    public override void Awake()
    {
        base.Awake();

        SingleHitDetector hitDetector = new(eventManager, hitbox, (hit) =>
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
            walkBehaviour.Stop();
        };

        onStart += () =>
        {
            isMoving = true;
            float direction = Mathf.Sign(movableObject.velocity.x);
            movableObject.velocity.x = direction * burstVelocity;
            movableObject.velocity.z = 0;
            movableObject.acceleration.x = -direction * burstAcceleration;
            eventManager.Attach(() => Mathf.Sign(movableObject.velocity.x) != direction, () => isMoving = false);
            hitDetector.Start();
        };

        void FinishAction()
        {
            movableObject.velocity.x = 0;
            movableObject.acceleration.x = 0;
            hitDetector.Stop();
        }

        onFinish += FinishAction;

        onStop += FinishAction;
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitWhile(() => isMoving);
    }

    protected override float CalculateDamage(HittableBehaviour hittableBehaviour)
    {
        return damage;
    }
}
