/// <summary>
/// Base class for energy suppliers
/// </summary>
public class BaseEnergySupplier : CharacterBehaviour
{
    protected EnergySystem energySystem;

    protected override void Awake()
    {
        base.Awake();
        energySystem = GetBehaviour<EnergySystem>();
    }
}