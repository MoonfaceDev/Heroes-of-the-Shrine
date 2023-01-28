using UnityEngine;

/// <summary>
/// Physical attributes of a character
/// </summary>
[CreateAssetMenu(menuName = "Character Attributes/Physical Attributes")]
public class PhysicalAttributes : ScriptableObject
{
    /// <value>
    /// The rate in which the character gains speed in Y axis when Y > 0
    /// </value>
    public float gravityAcceleration = 20;
}