public class SuperArmorHittableHitbox : HittableHitbox
{
    public float damageMultiplier = 1;

    private SuperArmorEffect superArmorEffect;

    protected override void Awake()
    {
        base.Awake();
        superArmorEffect = hittableBehaviour.GetComponent<SuperArmorEffect>();
    }

    public override void Hit(float damage)
    {
        if (superArmorEffect.Active)
        {
            superArmorEffect.HitArmor(damage);
            hittableBehaviour.Hit(damage * damageMultiplier);
        }
        else
        {
            hittableBehaviour.Hit(damage);
        }
    }

    public override void Knockback(float damage, float power, float angleDegrees)
    {
        if (superArmorEffect.Active)
        {
            Hit(damage);
        }
        else
        {
            hittableBehaviour.Knockback(damage, power, angleDegrees);
        }
    }

    public override void Stun(float damage, float time)
    {
        if (superArmorEffect.Active)
        {
            Hit(damage);
        }
        else
        {
            hittableBehaviour.Stun(damage, time);
        }
    }
}