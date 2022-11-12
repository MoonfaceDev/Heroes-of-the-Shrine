using System.Collections.Generic;
using System.Linq;

public interface IModifier
{
    float Modify(float value);
}

public class FlatModifier : IModifier
{
    private readonly float increment;

    public FlatModifier(float increment)
    {
        this.increment = increment;
    }

    public float Modify(float value)
    {
        return value + increment;
    }
}

public class MultiplierModifier : IModifier
{
    private readonly float factor;

    public MultiplierModifier(float factor)
    {
        this.factor = factor;
    }

    public float Modify(float value)
    {
        return value * factor;
    }
}

public class ModifiableValue
{
    private float Value
    {
        get
        {
            var result = flatModifiers.Aggregate(value, (current, modifier) => modifier.Modify(current));
            return multiplierModifiers.Cast<IModifier>().Aggregate(result, (current, modifier) => modifier.Modify(current));
        }
    }

    private readonly float value;
    private readonly List<FlatModifier> flatModifiers;
    private readonly List<MultiplierModifier> multiplierModifiers;

    public ModifiableValue(float value)
    {
        this.value = value;
        flatModifiers = new List<FlatModifier>();
        multiplierModifiers = new List<MultiplierModifier>();
    }

    public static implicit operator ModifiableValue(float value)
    {
        return new ModifiableValue(value);
    }

    public static implicit operator float(ModifiableValue value)
    {
        return value.Value;
    }

    public void AddModifier(IModifier modifier)
    {
        switch (modifier)
        {
            case FlatModifier flatModifier:
                flatModifiers.Add(flatModifier);
                break;
            case MultiplierModifier multiplierModifier:
                multiplierModifiers.Add(multiplierModifier);
                break;
        }
    }

    public void RemoveModifier(IModifier modifier)
    {
        switch (modifier)
        {
            case FlatModifier flatModifier:
                flatModifiers.Remove(flatModifier);
                break;
            case MultiplierModifier multiplierModifier:
                multiplierModifiers.Remove(multiplierModifier);
                break;
        }
    }
}
