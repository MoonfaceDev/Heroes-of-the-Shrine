using System;
using UnityEngine;

[Serializable]
public class Rotation
{
    [SerializeField] private int value;

    public static Rotation Left => new(-1);
    public static Rotation Right => new(1);

    public Rotation(int value)
    {
        this.value = value;
    }

    private Rotation() : this(-1)
    {
    }

    public bool Equals(Rotation other)
    {
        return value == other.value;
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