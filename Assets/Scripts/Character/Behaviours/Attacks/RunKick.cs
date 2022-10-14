using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
public class RunKick : SimpleAttack
{
    public float velocity;
    public float acceleration;

    private bool isMoving = false;

    public override void Awake()
    {
        base.Awake();
        float direction = 0;

        onAnticipate += () =>
        {
            direction = Mathf.Sign(movableObject.velocity.x);
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop();
        };

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

    public override bool CanAttack()
    {
        RunBehaviour runBehaviour = GetComponent<RunBehaviour>();
        return base.CanAttack() && (runBehaviour && runBehaviour.run);
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitWhile(() => isMoving);
    }
}
