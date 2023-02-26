public class RunSwipe : SimpleAttack
{
    protected override MotionSettings Motion => MotionSettings.WalkingDisabled;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && IsPlaying<RunBehaviour>();
    }
}