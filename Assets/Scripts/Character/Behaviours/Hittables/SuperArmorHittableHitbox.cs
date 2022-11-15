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
        superArmorEffect.HitArmor(damage);
    }

    public override void Knockback(float damage, float power, float angleDegrees, float stunTime)
    {
        Hit(damage);
    }

    public override void Stun(float damage, float time)
    {
        Hit(damage);
    }
}