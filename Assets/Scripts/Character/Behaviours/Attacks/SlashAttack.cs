using System.Collections;
using UnityEngine;

public class SlashAttack : SimpleAttack 
{
    public float velocity;
    public float acceleration;

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
            MovableObject.velocity.x = direction * velocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * acceleration;
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
