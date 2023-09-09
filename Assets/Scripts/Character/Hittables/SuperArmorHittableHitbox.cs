/// <summary>
/// <see cref="HittableHitbox"/> that cannot receive knockback or stun, and proxies hits to <see cref="superArmor"/>
/// </summary>
public class SuperArmorHittableHitbox : HittableHitbox
{
    private SuperArmor superArmor;
    private HealthSystem healthSystem;

    protected override void Awake()
    {
        base.Awake();
        superArmor = RelatedEntity.GetBehaviour<SuperArmor>();
        healthSystem = RelatedEntity.GetBehaviour<HealthSystem>();
    }

    public override void ProcessHit(IHitExecutor executor, Hit hit)
    {
        switch (executor)
        {
            case KnockbackHitExecutor or StunHitExecutor:
                return;
            case DamageHitExecutor hitExecutor:
                var processedDamage = hit.Source.AttackManager.damageTranspiler.Transpile(hit.Source, hit.Victim, hitExecutor.damage);
                superArmor.HitArmor(processedDamage / healthSystem.damageMultiplier);
                return;
        }

        base.ProcessHit(executor, hit);
    }
}