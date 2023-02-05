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
    public float Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, 0, maxEnergy);
    }

    /// <value>
    /// Ratio between current energy to full energy
    /// </value>
    public float Fraction => Energy / maxEnergy;

    private float energy;
}