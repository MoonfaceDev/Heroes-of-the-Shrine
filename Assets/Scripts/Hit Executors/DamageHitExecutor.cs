using System;

/// <summary>
/// Hit executor that deals damage to an hittable
/// </summary>
[Serializable]
public class DamageHitExecutor : IHitExecutor
{
    /// <value>
    /// Health reduced to hit characters
    /// </value>
    public float damage;

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        var processedDamage = attack.AttackManager.damageTranspiler.Transpile(attack, hittable, damage);
        hittable.Hit(processedDamage);
    }
}