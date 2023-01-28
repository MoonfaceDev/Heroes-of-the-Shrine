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
    
    /// <summary>
    /// Deals damage
    /// </summary>
    /// <param name="damage">Amount of damage, including the attacker's bonuses</param>
    public void Hit(float damage);

    /// <summary>
    /// Apply knockback
    /// </summary>
    /// <param name="power">Knockback power, affects launch speed</param>
    /// <param name="angleDegrees">Angle in which hittable is launched, in degrees</param>
    /// <param name="stunTime">Duration of stun, used for characters resistant to knockback</param>
    public void Knockback(float power, float angleDegrees, float stunTime = 0);

    /// <summary>
    /// Apply stun
    /// </summary>
    /// <param name="time">Duration of stun</param>
    public void Stun(float time);
}