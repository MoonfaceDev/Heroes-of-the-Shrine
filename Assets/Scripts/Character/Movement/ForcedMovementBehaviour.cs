public interface IForcedBehaviour : IPlayableBehaviour
{
}

public abstract class ForcedBehaviour<T> : PlayableBehaviour<T>, IForcedBehaviour
{
}