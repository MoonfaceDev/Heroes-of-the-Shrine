using UnityEngine;

public class SuperArmorEffect : BaseEffect
{
    public float armorHealth;
    public float armorCooldown;
    public float damageMultiplier = 1;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public override void Awake()
    {
        base.Awake();
        InitializeArmor();
    }

    private void InitializeArmor()
    {
        currentArmorHealth = armorHealth;
        Active = true;
        DisableBehaviours(typeof(KnockbackBehaviour), typeof(StunBehaviour));
        GetComponent<HittableBehaviour>().damageMultiplier *= damageMultiplier;
    }

    private void CancelArmor()
    {
        currentArmorHealth = 0;
        Active = false;
        EnableBehaviours(typeof(KnockbackBehaviour), typeof(StunBehaviour));
        GetComponent<HittableBehaviour>().damageMultiplier /= damageMultiplier;
    }

    public void HitArmor(float damage)
    {
        if (!Active) return;
        currentArmorHealth = Mathf.Max(currentArmorHealth - damage, 0);
        if (currentArmorHealth == 0)
        {
            CancelArmor();
            armorCooldownStart = Time.time;
            EventManager.StartTimeout(InitializeArmor, armorCooldown);
        }
    }

    public override void Stop()
    {
        CancelArmor();
    }

    public override float GetProgress()
    {
        return currentArmorHealth / armorHealth;
    }
}