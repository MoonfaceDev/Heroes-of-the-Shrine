using System;
using UnityEngine;

/// <summary>
/// Rotation that has two options - normal or flipped
/// </summary>
[Serializable]
public class Rotation
{
    [SerializeField] private bool flipped;

    public static Rotation Normal => new(false);
    public static Rotation Flipped => new(true);

    private Rotation(bool flipped)
    {
        this.flipped = flipped;
    }

    private Rotation() : this(false)
    {
    }

    public static bool operator ==(Rotation a, Rotation b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.flipped == b.flipped;
    }

    public static bool operator !=(Rotation a, Rotation b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        // 'flipped' can only be modified by the inspector, therefore we can use its hash value
        return flipped.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (GetType() != obj.GetType()) return false;
        return flipped == ((Rotation)obj).flipped;
    }

    public static Vector3 operator *(Rotation rotation, Vector3 relativePosition)
    {
        return Vector3.Scale(relativePosition, new Vector3((int)rotation, 1, 1));
    }

    /// <summary>
    /// The opposite rotation
    /// </summary>
    public static Rotation operator -(Rotation rotation)
    {
        return new Rotation(!rotation.flipped);
    }

    public static implicit operator Quaternion(Rotation rotation)
    {
        return Quaternion.Euler(0, (1 - (int)rotation) * 90, 0);
    }

    public static implicit operator int(Rotation rotation)
    {
        return rotation.flipped ? -1 : 1;
    }

    public static implicit operator Rotation(int value)
    {
        return new Rotation(value == -1);
    }
    
    public static implicit operator Rotation(Vector3 offset)
    {
        return new Rotation(offset.x < 0);
    }
}