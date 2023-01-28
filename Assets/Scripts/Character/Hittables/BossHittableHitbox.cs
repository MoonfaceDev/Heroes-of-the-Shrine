/// <summary>
/// <see cref="HittableHitbox"/> that converts knockback to stun
/// </summary>
public class BossHittableHitbox : HittableHitbox
{
    public override void Knockback(float power, float angleDegrees, float stunTime)
    {
        base.Stun(stunTime);
    }
}