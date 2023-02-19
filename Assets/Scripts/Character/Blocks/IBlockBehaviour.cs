public interface IBlockBehaviour : IControlledBehaviour
{
    public bool TryBlock(Hit hit);
}