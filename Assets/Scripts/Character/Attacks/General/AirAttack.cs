using System.Collections;

/// <summary>
/// Attack that can be only be played when jumping, and does not prevent walking
/// </summary>
public class AirAttack : SimpleAttack
{
    public float minHeight;

    protected override MotionSettings Motion => MotionSettings.WalkingEnabled;
    protected override bool IsMidair => true;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && Entity.position.y > minHeight;
    }

    protected override IEnumerator ActivePhase()
    {
        var jumpBehaviour = GetBehaviour<JumpBehaviour>();

        jumpBehaviour.Freeze();
        yield return base.ActivePhase();
        jumpBehaviour.Unfreeze();
    }
}