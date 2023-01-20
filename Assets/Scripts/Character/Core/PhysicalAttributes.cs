using UnityEngine;

/// <summary>
/// Physical attributes of a character
/// </summary>
[CreateAssetMenu(menuName = "Character Attributes/Physical Attributes")]
public class PhysicalAttributes : ScriptableObject
{
    public float gravityAcceleration = 20;
}