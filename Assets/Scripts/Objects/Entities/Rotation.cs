using UnityEngine;

public class Rotation
{
    private readonly int value;

    public static readonly Rotation Left = new(-1);
    public static readonly Rotation Right = new(1);

    private Rotation(int value)
    {
        this.value = value;
    }

    public static bool operator ==(Rotation a, Rotation b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (ReferenceEquals(a, null))
            return false;
        if (ReferenceEquals(b, null))
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(Rotation a, Rotation b) => !(a == b);

    private bool Equals(Rotation other)
    {
        if (ReferenceEquals(other, null))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return value.Equals(other.value);
    }

    public override bool Equals(object obj) => Equals(obj as Rotation);

    public override int GetHashCode()
    {
        return value;
    }

    public static Rotation operator *(Rotation a, Rotation b)
    {
        return new Rotation(a.value * b.value);
    }

    public static Vector3 operator *(Rotation rotation, Vector3 relativePosition)
    {
        return Vector3.Scale(relativePosition, new Vector3(rotation.value, 1, 1));
    }

    public static Rotation operator -(Rotation rotation)
    {
        return new Rotation(-rotation.value);
    }

    public static implicit operator Quaternion(Rotation rotation)
    {
        return Quaternion.Euler(0, (1 - rotation.value) * 90, 0);
    }

    public static implicit operator int(Rotation rotation)
    {
        return rotation.value;
    }

    public static implicit operator Rotation(int value)
    {
        return new Rotation(value);
    }
}