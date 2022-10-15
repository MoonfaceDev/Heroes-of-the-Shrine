using System.Collections.Generic;

public interface IModifier
{
    float Modify(float value);
}

public class FlatModifier : IModifier
{
    public float increment;

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
    public float factor;

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
    public float value
    {
        get
        {
            float result = _value;
            foreach (IModifier modifier in flatModifiers)
            {
                result = modifier.Modify(result);
            }
            foreach (IModifier modifier in multiplierModifiers)
            {
                result = modifier.Modify(result);
            }
            return result;
        }
    }

    private float _value;
    private List<FlatModifier> flatModifiers;
    private List<MultiplierModifier> multiplierModifiers;

    public ModifiableValue(float value)
    {
        _value = value;
    }

    public static implicit operator ModifiableValue(float value)
    {
        return new ModifiableValue(value);
    }

    public static implicit operator float(ModifiableValue value)
    {
        return value;
    }

    public void AddModifier(IModifier modifier)
    {
        if (modifier is FlatModifier)
        {
            flatModifiers.Add(modifier as FlatModifier);
        }
        if (modifier is MultiplierModifier)
        {
            multiplierModifiers.Add(modifier as MultiplierModifier);
        }
    }

    public void RemoveModifier(IModifier modifier)
    {
        if (modifier is FlatModifier)
        {
            flatModifiers.Remove(modifier as FlatModifier);
        }
        if (modifier is MultiplierModifier)
        {
            multiplierModifiers.Remove(modifier as MultiplierModifier);
        }
    }
}
