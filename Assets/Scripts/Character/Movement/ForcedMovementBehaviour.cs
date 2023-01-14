public interface IForcedBehaviour : IPlayableBehaviour
{
}

public abstract class ForcedBehaviour<T> : PlayableBehaviour<T>, IForcedBehaviour where T : ICommand
{
}