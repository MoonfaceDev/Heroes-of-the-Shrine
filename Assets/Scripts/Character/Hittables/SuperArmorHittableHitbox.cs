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
        superArmor = Character.GetBehaviour<SuperArmor>();
        healthSystem = Character.GetBehaviour<HealthSystem>();
    }

    public override bool Hit(IHitExecutor executor, Hit hit)
    {
        switch (executor)
        {
            case KnockbackHitExecutor or StunHitExecutor:
                return false;
            case DamageHitExecutor hitExecutor:
                var processedDamage = hit.source.AttackManager.damageTranspiler.Transpile(hit.source, hit.victim, hitExecutor.damage);
                superArmor.HitArmor(processedDamage / healthSystem.damageMultiplier);
                return true;
        }

        return base.Hit(executor, hit);
    }
}