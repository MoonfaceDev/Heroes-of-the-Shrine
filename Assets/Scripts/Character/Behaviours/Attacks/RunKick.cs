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

        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop();
        };

        onStart += () =>
        {
            isMoving = true;
            float direction = Mathf.Sign(movableObject.velocity.x);
            movableObject.velocity.x = direction * velocity;
            movableObject.velocity.z = 0;
            movableObject.acceleration.x = -direction * acceleration;
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
