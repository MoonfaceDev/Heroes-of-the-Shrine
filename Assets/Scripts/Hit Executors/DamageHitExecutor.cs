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

    public void Execute(Hit hit)
    {
        var processedDamage = hit.source != null
            ? hit.source.AttackManager.damageTranspiler.Transpile(hit.source, hit.victim, damage)
            : damage;
        hit.victim.Character.GetBehaviour<HealthSystem>().Hit(processedDamage);
    }
}