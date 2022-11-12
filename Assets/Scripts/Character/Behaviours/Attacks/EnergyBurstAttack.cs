using System.Collections;
using UnityEngine;

public class EnergyBurstAttack : SimpleAttack
{
    public float burstVelocity;
    public float burstAcceleration;

    private bool isMoving;

    public override void Awake()
    {
        base.Awake();

        PreventWalking(false);

        var direction = 0;
        OnPlay += () => direction = LookDirection;

        OnStartActive += () =>
        {
            isMoving = true;
            MovableObject.velocity.x = direction * burstVelocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * burstAcceleration;
            EventManager.Attach(() => MovableObject.velocity.x == 0 || Mathf.RoundToInt(Mathf.Sign(MovableObject.velocity.x)) != direction, () => isMoving = false);
        };

        OnFinishActive += () => 
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
