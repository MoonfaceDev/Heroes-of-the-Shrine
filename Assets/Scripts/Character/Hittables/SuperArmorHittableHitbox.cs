/// <summary>
/// <see cref="HittableHitbox"/> that cannot receive knockback or stun, and proxies hits to <see cref="superArmorEffect"/>
/// </summary>
public class SuperArmorHittableHitbox : HittableHitbox
{
    private SuperArmorEffect superArmorEffect;

    protected override void Awake()
    {
        base.Awake();
        superArmorEffect = hittableBehaviour.GetComponent<SuperArmorEffect>();
    }

    public override void Hit(float damage)
    {
        if (!CanGetHit())
        {
            return;
        }
        Blink();
        superArmorEffect.HitArmor(damage);
    }

    public override void Knockback(float power, float angleDegrees, float stunTime)
    {
    }

    public override void Stun(float time)
    {
    }
}