/// <summary>
/// interface for objects that can get hit
/// </summary>
public interface IHittable
{
    /// <value>
    /// Related character
    /// </value>
    public Character Character { get; }

    /// <summary>
    /// Checks if the hittable can take hits at the moment
    /// </summary>
    /// <returns><c>true</c> if it can take hits</returns>
    public bool CanGetHit();
    
    public void Hit(float damage);

    public void Knockback(float damage, float power, float angleDegrees, float stunTime = 0);

    public void Stun(float damage, float time);
}