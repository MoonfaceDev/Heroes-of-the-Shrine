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

    public void AddEnergy(float amount)
    {
        amount = Mathf.Clamp(amount, 0, maxEnergy - Energy);
        Energy += amount;
        onEnergyGrow.Invoke(amount);
    }

    public void TakeEnergy(float amount)
    {
        amount = Mathf.Clamp(amount, 0, Energy);
        Energy -= amount;
    }

    public void ResetEnergy()
    {
        Energy = 0;
    }
}