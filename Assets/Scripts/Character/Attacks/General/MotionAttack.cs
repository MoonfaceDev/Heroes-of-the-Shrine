using System.Collections;
using UnityEngine;

/// <summary>
/// An attack that has a single hit detector, hit executor, and has an anticipation and recovery phases with fixed
/// duration. During the active phase, the character moves in the direction it looked at with a reducing speed, until it
/// reaches zero.
/// </summary>
public class MotionAttack : BaseAttack
{
    public float velocity;
    public float acceleration;
    
    public HitSource hitSource;

    protected override MotionSettings Motion => MotionSettings.WalkingDisabled;

    private void Start()
    {
        phaseEvents.onFinishActive += FinishActive;
    }

    private void FinishActive()
    {
        hitSource.Stop();
        MovableEntity.velocity.x = 0;
        MovableEntity.acceleration.x = 0;
    }

    protected override IEnumerator ActivePhase()
    {
        var originalDirection = MovableEntity.rotation;
        MovableEntity.velocity.x = originalDirection * velocity;
        MovableEntity.velocity.z = 0;
        MovableEntity.acceleration.x = -originalDirection * acceleration;

        hitSource.Start(this);

        yield return new WaitUntil(() =>
            Mathf.Approximately(MovableEntity.velocity.x, 0) ||
            !Mathf.Approximately(Mathf.Sign(MovableEntity.velocity.x), originalDirection)
        );
    }
}