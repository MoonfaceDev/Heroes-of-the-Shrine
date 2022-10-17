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

        PreventWalking(false);

        float direction = 0;
        onPlay += () => direction = lookDirection;

        onStart += () =>
        {
            isMoving = true;
            movableObject.velocity.x = direction * burstVelocity;
            movableObject.velocity.z = 0;
            movableObject.acceleration.x = -direction * burstAcceleration;
            eventManager.Attach(() => Mathf.Sign(movableObject.velocity.x) != direction, () => isMoving = false);
        };

        onFinish += () => 
        {
            movableObject.velocity.x = 0;
            movableObject.acceleration.x = 0;
        };
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitWhile(() => isMoving);
    }
}
