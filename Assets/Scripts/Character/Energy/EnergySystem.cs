using ExtEvents;
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

    public ExtEvent<float> onEnergyGrow;

    /// <value>
    /// Current energy value
    /// </value>
    public float Energy { get; private set; }

    /// <value>
    /// Ratio between current energy to full energy
    /// </value>
    public float Fraction => Energy / maxEnergy;

    public bool Full => Mathf.Approximately(Fraction, 1);

    public void AddEnergy(float energyAddition)
    {
        energyAddition = Mathf.Clamp(energyAddition, 0, maxEnergy - Energy);
        Energy += energyAddition;
        onEnergyGrow.Invoke(energyAddition);
    }
}