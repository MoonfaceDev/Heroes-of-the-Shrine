using System;

/// <summary>
/// Hit side effect types
/// </summary>
public enum HitType
{
    Knockback,
    Stun
}

/// <summary>
/// Data structure for defining basic hit effects.
/// </summary>
[Serializable]
public class HitDefinition
{
    /// <value>
    /// Health reduced to hit characters
    /// </value>
    public float damage;
    
    /// <value>
    /// Side effect of the hit, either knockback or stun
    /// </value>
    public HitType hitType = HitType.Knockback;
    
    /// <value>
    /// Power of the knockback, affects its initial speed
    /// </value>
    public float knockbackPower;

    /// <value>
    /// Direction of the knockback in degrees, relative to X axis, in the direction of the hit
    /// </value>
    public float knockbackDirection;

    /// <value>
    /// Duration of stun effect caused by hit.
    /// If enemy is resistant to knockback, this value will be used too.
    /// </value>
    public float stunTime = 0.5f;
}