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
        var processedDamage = hit.Source != null
            ? hit.Source.AttackManager.damageTranspiler.Transpile(hit.Source, hit.Victim, damage)
            : damage;
        hit.Victim.RelatedEntity.GetBehaviour<HealthSystem>().Hit(processedDamage);
    }
}