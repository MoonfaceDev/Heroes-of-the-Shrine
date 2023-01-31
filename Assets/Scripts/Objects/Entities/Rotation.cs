using System;
using UnityEngine;

[Serializable]
public class Rotation
{
    public enum Value
    {
        Left = -1,
        Right = 1,
    }

    [SerializeField] private Value value;

    public static Rotation Left => new(Value.Left);

    public static Rotation Right => new(Value.Right);

    public Rotation(Value value)
    {
        this.value = value;
    }

    private Rotation() : this(Value.Left)
    {
    }

    public static bool operator ==(Rotation a, Rotation b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.value == b.value;
    }

    public static bool operator !=(Rotation a, Rotation b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        // 'value' can only be modified by the inspector, therefore we can use its hash value
        return value.GetHashCode();
    }
    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (GetType() != obj.GetType()) return false;
        return value == ((Rotation) obj).value;
    }

    public static Vector3 operator *(Rotation rotation, Vector3 relativePosition)
    {
        return Vector3.Scale(relativePosition, new Vector3((int)rotation.value, 1, 1));
    }

    public static Rotation operator -(Rotation rotation)
    {
        return new Rotation((Value)(-(int)rotation.value));
    }

    public static implicit operator Quaternion(Rotation rotation)
    {
        return Quaternion.Euler(0, (1 - (int)rotation.value) * 90, 0);
    }

    public static implicit operator int(Rotation rotation)
    {
        return (int)rotation.value;
    }

    public static implicit operator Rotation(int value)
    {
        return new Rotation((Value)value);
    }
}