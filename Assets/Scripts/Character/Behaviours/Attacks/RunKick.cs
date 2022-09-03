using System.Collections;
using UnityEngine;

public class RunKick : SimpleAttack
{
    public Hitbox hitbox;
    public float velocity;
    public float acceleration;

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
            walkBehaviour.Stop();
        };
    }

    protected override bool CanAttack()
    {
        RunBehaviour runBehaviour = GetComponent<RunBehaviour>();
        return base.CanAttack() && (runBehaviour && runBehaviour.run);
    }

    protected override IEnumerator ActiveCoroutine()
    {
        bool stopped = false;
        float direction = Mathf.Sign(movableObject.velocity.x);
        movableObject.velocity.x = direction * velocity;
        movableObject.acceleration.x = -direction * acceleration;
        eventManager.Attach(() => Mathf.Sign(movableObject.velocity.x) != direction, () => stopped = true);
        hitDetector.Start();
        yield return new WaitUntil(() => stopped);
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
        hitDetector.Stop();
    }
}
