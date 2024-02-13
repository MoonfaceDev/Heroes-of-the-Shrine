public class RunSwipe : SimpleAttack
{
    [InjectBehaviour] private WalkBehaviour walkBehaviour;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && IsPlaying<WalkBehaviour>() && IsPlaying<RunBehaviour>();
    }

    protected override void DoPlay(Command command)
    {
        base.DoPlay(command);
        MovableEntity.velocity.x = walkBehaviour.speed * Entity.rotation;
    }

    protected override void DoStop()
    {
        base.DoStop();
        MovableEntity.velocity.x = 0;
    }
}