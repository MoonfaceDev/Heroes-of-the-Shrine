/// <summary>
/// <see cref="HittableHitbox"/> that cannot receive knockback or stun, and proxies hits to <see cref="superArmor"/>
/// </summary>
public class SuperArmorHittableHitbox : HittableHitbox
{
    private SuperArmor superArmor;

    protected override void Awake()
    {
        base.Awake();
        superArmor = Character.GetComponent<SuperArmor>();
    }

    public override void Hit(float damage)
    {
        if (!CanGetHit())
        {
            return;
        }
        Blink();
        superArmor.HitArmor(damage);
    }

    public override void Knockback(float power, float angleDegrees, float stunTime)
    {
    }

    public override void Stun(float time)
    {
    }
}