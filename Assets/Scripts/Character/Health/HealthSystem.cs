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
    public float Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, startHealth);
    }

    /// <value>
    /// Ratio between current health to full health
    /// </value>
    public float Fraction => Health / startHealth;

    private float health;

    protected override void Awake()
    {
        base.Awake();
        Health = startHealth;
    }

    /// <summary>
    /// Kill character by setting health to 0
    /// </summary>
    public void Kill()
    {
        Health = 0;
    }

    /// <value>
    /// Is character alive
    /// </value>
    public bool Alive => Health > 0;
}