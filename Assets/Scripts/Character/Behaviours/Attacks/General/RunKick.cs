using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
public class RunKick : SimpleAttack
{
    public float velocity;
    public float acceleration;

    private bool isMoving;

    public override void Awake()
    {
        base.Awake();

        PreventWalking(false);

        var direction = 0;
        onPlay.AddListener(() => direction = MovableObject.rotation);

        generalEvents.onStartActive.AddListener(() =>
        {
            isMoving = true;
            MovableObject.velocity.x = direction * velocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * acceleration;
            EventManager.Attach(() => MovableObject.velocity.x == 0 || Mathf.RoundToInt(Mathf.Sign(MovableObject.velocity.x)) != direction, () => isMoving = false);
        });

        generalEvents.onFinishActive.AddListener(() => 
        {
            MovableObject.velocity.x = 0;
            MovableObject.acceleration.x = 0;
        });
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
