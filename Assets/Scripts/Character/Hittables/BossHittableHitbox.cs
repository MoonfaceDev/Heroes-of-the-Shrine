/// <summary>
/// <see cref="HittableHitbox"/> that converts knockback to stun
/// </summary>
public class BossHittableHitbox : HittableHitbox
{
    public override void ProcessHit(IHitExecutor executor, Hit hit)
    {
        if (executor is KnockbackHitExecutor)
        {
            return;
        }
        base.ProcessHit(executor, hit);
    }
}