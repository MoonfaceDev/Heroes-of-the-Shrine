public class BossHittableHitbox : HittableHitbox
{
    public override void Knockback(float damage, float power, float angleDegrees, float stunTime)
    {
        base.Stun(damage, stunTime);
    }
}