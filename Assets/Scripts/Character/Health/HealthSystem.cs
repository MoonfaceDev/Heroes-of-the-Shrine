using ExtEvents;
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
        private set => health = Mathf.Clamp(value, 0, startHealth);
    }
    
    /// <value>
    /// Multiplier for any damage that character is getting
    /// </value>
    [ShowDebug] public float damageMultiplier = 1;

    /// <value>
    /// Invoked when <see cref="Hit"/> is called
    /// </value>
    [SerializeField] public ExtEvent onHit;

    /// <value>
    /// Ratio between current health to full health
    /// </value>
    public float Fraction => Health / startHealth;

    private float health;

    protected override void Awake()
    {
        base.Awake();
        Health = startHealth;
        damageMultiplier = 1;
    }
    
    private float ProcessDamage(float damage)
    {
        return damage * damageMultiplier;
    }

    public void Hit(float damage)
    {
        var processedDamage = ProcessDamage(damage);
        Health -= processedDamage;
        onHit.Invoke();
    }

    public void Heal(float amount)
    {
        Health += amount;
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