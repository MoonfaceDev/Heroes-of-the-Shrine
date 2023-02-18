/// <summary>
/// <see cref="HittableHitbox"/> that converts knockback to stun
/// </summary>
public class BossHittableHitbox : HittableHitbox
{
    public override bool Hit(IHitExecutor executor, Hit hit)
    {
        if (executor is KnockbackHitExecutor)
        {
            return false;
        }
        return base.Hit(executor, hit);
    }
}