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

        PreventWalking(false);

        float direction = 0;
        OnPlay += () => direction = LookDirection;

        OnStartActive += () =>
        {
            isMoving = true;
            MovableObject.velocity.x = direction * velocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * acceleration;
            EventManager.Attach(() => MovableObject.velocity.x == 0 || Mathf.Sign(MovableObject.velocity.x) != direction, () => isMoving = false);
        };

        OnFinishActive += () => 
        {
            MovableObject.velocity.x = 0;
            MovableObject.acceleration.x = 0;
        };
    }

    public override bool CanPlay()
    {
        return base.CanPlay() && IsPlaying(typeof(RunBehaviour));
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitWhile(() => isMoving);
    }
}
