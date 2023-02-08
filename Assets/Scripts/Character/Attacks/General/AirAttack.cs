using System.Collections;

/// <summary>
/// Attack that can be only be played when jumping, and does not prevent walking
/// </summary>
public class AirAttack : SimpleAttack
{
    protected override MotionSettings Motion => MotionSettings.WalkingEnabled;
    protected override bool IsMidair => true;

    protected override void DoPlay(Command command)
    {
        base.DoPlay(command);
    }
    
    
}