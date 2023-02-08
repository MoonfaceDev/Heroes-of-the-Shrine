using System.Collections;

/// <summary>
/// Attack that can be only be played when jumping, and does not prevent walking
/// </summary>
public class AirAttack : SimpleAttack
{
    protected override MotionSettings Motion => MotionSettings.WalkingEnabled;
    protected override bool IsMidair => true;

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.velocity.y = 0;
        MovableEntity.acceleration.y = 0;
        yield return base.ActivePhase();
        MovableEntity.acceleration.y = -0.5f * Character.physicalAttributes.gravityAcceleration;
    }
}