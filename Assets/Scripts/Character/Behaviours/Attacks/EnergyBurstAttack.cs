using System.Collections;
using UnityEngine;

public class EnergyBurstAttack : SimpleAttack
{
    public float burstVelocity;
    public float burstAcceleration;

    private bool isMoving = false;

    public override void Awake()
    {
        base.Awake();

        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            if (walkBehaviour)
            {
                walkBehaviour.Stop();
            }
        };

        onStart += () =>
        {
            isMoving = true;
            float direction = Mathf.Sign(movableObject.velocity.x);
            movableObject.velocity.x = direction * burstVelocity;
            movableObject.velocity.z = 0;
            movableObject.acceleration.x = -direction * burstAcceleration;
            eventManager.Attach(() => Mathf.Sign(movableObject.velocity.x) != direction, () => isMoving = false);
        };

        void FinishAction()
        {
            movableObject.velocity.x = 0;
            movableObject.acceleration.x = 0;
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
