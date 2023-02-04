public class RunKick : MotionAttack
{
    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && IsPlaying<RunBehaviour>();
    }
}