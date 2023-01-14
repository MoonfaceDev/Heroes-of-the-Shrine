/// <summary>
/// Interface for hit executors
/// </summary>
public interface IHitExecutor
{
    /// <summary>
    /// Performs hit logic on an object
    /// </summary>
    /// <param name="attack">Related attack that made hit</param>
    /// <param name="hittable">Object hit by the attack</param>
    public void Execute(BaseAttack attack, IHittable hittable);
}