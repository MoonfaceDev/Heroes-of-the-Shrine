using System.Collections;
using UnityEngine;

public class SlashAttack : SimpleAttack 
{
    public float velocity;
    public float acceleration;

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
            movableObject.velocity.x = direction * velocity;
            movableObject.velocity.z = 0;
            movableObject.acceleration.x = -direction * acceleration;
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
