using ExtEvents;

/// <summary>
/// interface for objects that can get hit
/// </summary>
public interface IHittable
{
    /// <value>
    /// Related character
    /// </value>
    public Character Character { get; }

    /// <value>
    /// Invoked when <see cref="ProcessHit"/> is called
    /// </value>
    public ExtEvent HitEvent { get; }

    /// <summary>
    /// Checks if the hittable can take hits at the moment
    /// </summary>
    /// <returns><c>true</c> if it can take hits</returns>
    public bool CanGetHit();

    public void Hit(ChainHitExecutor executor, Hit hit);

    public void ProcessHit(IHitExecutor executor, Hit hit);
}