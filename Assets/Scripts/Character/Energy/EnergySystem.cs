using UnityEngine;

/// <summary>
/// Character energy system
/// </summary>
public class EnergySystem : CharacterBehaviour
{
    /// <value>
    /// Character's full energy value
    /// </value>
    public float maxEnergy;
    
    /// <value>
    /// Current energy value
    /// </value>
    [HideInInspector] public float energy;

    /// <value>
    /// Ratio between current energy to full energy
    /// </value>
    public float Fraction => energy / maxEnergy;
}