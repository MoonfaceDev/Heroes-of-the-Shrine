public class AirAttack : SimpleAttack
{
    protected override MotionSettings Motion => MotionSettings.WalkingEnabled;
    protected override bool IsMidair => true;
}