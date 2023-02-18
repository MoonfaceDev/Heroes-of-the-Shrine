public class Hit
{
    /// <value>
    /// Related attack that made hit
    /// </value>
    public BaseAttack source;

    /// <value>
    /// Hittable that got hit
    /// </value>
    public IHittable victim;
}

/// <summary>
/// Interface for hit executors
/// </summary>
public interface IHitExecutor
{
    /// <summary>
    /// Performs hit logic on an object
    /// </summary>
    /// <param name="hit">Hit context</param>
    public void Execute(Hit hit);
}