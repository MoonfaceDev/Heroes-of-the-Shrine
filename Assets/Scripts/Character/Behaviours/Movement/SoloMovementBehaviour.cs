public abstract class SoloMovementBehaviour : BaseMovementBehaviour
{
    public override bool CanPlay()
    {
        return base.CanPlay() && AllStopped(typeof(SoloMovementBehaviour), typeof(AttackManager));
    }
}
