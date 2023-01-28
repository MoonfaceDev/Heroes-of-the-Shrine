using UnityEngine;

/// <summary>
/// Character health system
/// </summary>
public class HealthSystem : CharacterBehaviour
{
    /// <value>
    /// Character's full health value
    /// </value>
    public float startHealth;
    
    /// <value>
    /// Current health value
    /// </value>
    [HideInInspector] public float health;

    /// <value>
    /// Ratio between current health to full health
    /// </value>
    public float Fraction => health / startHealth;

    protected override void Awake()
    {
        base.Awake();
        health = startHealth;
    }

    /// <summary>
    /// Kill character by setting health to 0
    /// </summary>
    public void Kill()
    {
        health = 0;
    }

    /// <value>
    /// Is character alive
    /// </value>
    public bool Alive => health > 0;
}