using UnityEngine;

public class SuperArmorEffect : BaseEffect
{
    public float armorHealth;
    public float armorCooldown;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public override void Awake()
    {
        base.Awake();
        currentArmorHealth = armorHealth;
        Active = true;
    }

    public void HitArmor(float damage)
    {
        currentArmorHealth = Mathf.Max(currentArmorHealth - damage, 0);
        if (currentArmorHealth == 0)
        {
            Active = false;
            armorCooldownStart = Time.time;
            EventManager.StartTimeout(() =>
            {
                currentArmorHealth = armorHealth;
                Active = true;
            }, armorCooldown);
        }
    }
    
    public override void Stop()
    {
        currentArmorHealth = 0;
        Active = false;
    }

    public override float GetProgress()
    {
        return currentArmorHealth / armorHealth;
    }
}