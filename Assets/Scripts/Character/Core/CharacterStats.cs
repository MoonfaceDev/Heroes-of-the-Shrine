using UnityEngine;

/// <summary>
/// Stats of a character
/// </summary>
[CreateAssetMenu(menuName = "Character/Character Stats")]
public class CharacterStats : ScriptableObject
{
    /// <value>
    /// The rate in which the character gains speed in Y axis when Y > 0
    /// </value>
    public float gravityAcceleration = 1;

    public float damageMultiplier = 1;

    public float knockbackPowerMultiplier = 1;
}