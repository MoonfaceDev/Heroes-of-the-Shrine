using UnityEngine;

public class SuperArmorHittableBehaviour : HittableBehaviour
{
    public float damageMultiplier = 1;
    public float armorHealth;
    public float armorCooldown;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public bool ArmorActive => currentArmorHealth > 0;

    public float Fraction => currentArmorHealth / armorHealth;

    private void Start()
    {
        currentArmorHealth = armorHealth;
    }

    protected override float ProcessDamage(float damage)
    {
        return (ArmorActive ? damageMultiplier : 1) * damage;
    }

    private void HitArmor(float damage)
    {
        currentArmorHealth = Mathf.Max(currentArmorHealth - damage, 0);
        if (currentArmorHealth == 0)
        {
            armorCooldownStart = Time.time;
            EventManager.StartTimeout(() => currentArmorHealth = armorHealth, armorCooldown);
        }
    }

    protected override void DoKnockback(float damage, float power, float angleDegrees)
    {
        if (!ArmorActive)
        {
            base.DoKnockback(damage, power, angleDegrees);
        }
        else
        {
            HitArmor(damage);
        }
    }

    protected override void DoStun(float damage, float time)
    {
        if (!ArmorActive)
        {
            base.DoStun(damage, time);
        }
        else
        {
            HitArmor(damage);
        }
    }
}
