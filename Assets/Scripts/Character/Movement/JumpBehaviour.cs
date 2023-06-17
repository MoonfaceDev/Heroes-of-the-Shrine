using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class JumpBehaviour : PhasedBehaviour<JumpBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
    }

    [FormerlySerializedAs("jumpSpeed")] public float speed;

    [FormerlySerializedAs("jumpAnticipateTime")]
    public float anticipateTime;

    [FormerlySerializedAs("jumpRecoverTime")]
    public float recoverTime;

    public float climbAcceleration;
    public float peakDuration;
    public float peakAcceleration;

    private bool landed;
    private string stopClimbListener;
    private string stopPeakListener;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && !(AttackManager && !AttackManager.CanPlayAttack())
               && !Playing;
    }

    protected override void DoPlay(Command command)
    {
        landed = false;
        StopBehaviours(typeof(BaseAttack));
        BlockBehaviours(typeof(RunBehaviour));

        base.DoPlay(command);
    }

    protected override IEnumerator ActivePhase()
    {
        StartJump();
        yield return new WaitUntil(() => landed);
    }

    private void StartJump()
    {
        MovableEntity.velocity.y = speed;
        MovableEntity.acceleration.y = -climbAcceleration;

        stopClimbListener = InvokeWhen(() => MovableEntity.velocity.y < 0, () =>
        {
            MovableEntity.acceleration.y = -peakAcceleration;
            var peakStartTime = Time.time;
            stopPeakListener = InvokeWhen(
                () => Time.time - peakStartTime > peakDuration,
                () => MovableEntity.acceleration.y = -Character.stats.gravityAcceleration
            );
        });

        MovableEntity.OnLand += Land;
    }

    public void Freeze()
    {
        Cancel(stopClimbListener);
        Cancel(stopPeakListener);
        MovableEntity.velocity.y = 0;
        MovableEntity.acceleration.y = 0;
    }

    public void Unfreeze()
    {
        MovableEntity.acceleration.y = -Character.stats.gravityAcceleration;
    }

    private void Land()
    {
        MovableEntity.OnLand -= Land;
        landed = true;
    }

    protected override void DoStop()
    {
        UnblockBehaviours(typeof(RunBehaviour));

        if (Active)
        {
            Cancel(stopClimbListener);
            Cancel(stopPeakListener);
            MovableEntity.velocity.y = 0;
            MovableEntity.OnLand -= Land;
        }

        base.DoStop();
    }
}