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
        OnPlay += () => direction = LookDirection;

        OnStart += () =>
        {
            isMoving = true;
            MovableObject.velocity.x = direction * burstVelocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * burstAcceleration;
            EventManager.Attach(() => Mathf.Sign(MovableObject.velocity.x) != direction, () => isMoving = false);
        };

        OnFinish += () => 
        {
            MovableObject.velocity.x = 0;
            MovableObject.acceleration.x = 0;
        };
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitWhile(() => isMoving);
    }
}
