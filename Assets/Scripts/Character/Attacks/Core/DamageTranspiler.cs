using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class responsible for processing damage values for the character dealing damage
/// </summary>
public class DamageTranspiler
{
    /// <summary>
    /// Damage bonus delegate
    /// </summary>
    public delegate float DamageBonus(BaseAttack attack, IHittable hittable);

    private readonly List<DamageBonus> damageBonuses;
    private readonly List<DamageBonus> damageMultipliers;

    /// <summary>
    /// Initializes a <see cref="DamageTranspiler"/>
    /// </summary>
    public DamageTranspiler()
    {
        damageBonuses = new List<DamageBonus>();
        damageMultipliers = new List<DamageBonus>();
    }

    /// <summary>
    /// Processes the raw damage value and applies bonuses and multipliers
    /// </summary>
    /// <param name="attack">Played attack</param>
    /// <param name="hittable">Hittable for which damage is calculated</param>
    /// <param name="damage">Raw attack damage</param>
    /// <returns>Transpiled damage</returns>
    public float TranspileDamage(BaseAttack attack, IHittable hittable, float damage)
    {
        damage += damageBonuses.Sum(bonus => bonus(attack, hittable));
        return damageMultipliers.Aggregate(damage, (current, bonus) => current * bonus(attack, hittable));
    }

    /// <summary>
    /// Adds a new additive damage bonus
    /// </summary>
    /// <param name="bonus">Bonus function</param>
    public void AttachDamageBonus(DamageBonus bonus)
    {
        damageBonuses.Add(bonus);
    }

    /// <summary>
    /// Removes an additive damage bonus
    /// </summary>
    /// <param name="bonus">Reference to a bonus function that has been added</param>
    public void DetachDamageBonus(DamageBonus bonus)
    {
        damageBonuses.Remove(bonus);
    }

    /// <summary>
    /// Adds a new multiplicative damage bonus
    /// </summary>
    /// <param name="multiplier">Bonus function</param>
    public void AttachDamageMultiplier(DamageBonus multiplier)
    {
        damageMultipliers.Add(multiplier);
    }

    /// <summary>
    /// Removes a multiplicative damage bonus
    /// </summary>
    /// <param name="multiplier">Reference to a bonus function that has been added</param>
    public void DetachDamageMultiplier(DamageBonus multiplier)
    {
        damageMultipliers.Remove(multiplier);
    }
}