using UnityEngine;
using UnityEngine.Events;

public class SuperArmorEffect : BaseEffect
{
    public float armorHealth;
    public float armorCooldown;
    public float damageMultiplier = 1;
    public UnityEvent onHit;
    public UnityEvent onBreak;

    private EventListener reloadEvent;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public override void Awake()
    {
        base.Awake();
        InitializeArmor();
    }

    private void InitializeArmor()
    {
        Active = true;
        onPlay.Invoke();

        currentArmorHealth = armorHealth;

        GetComponent<HittableBehaviour>().damageMultiplier *= damageMultiplier;
        DisableBehaviours(typeof(KnockbackBehaviour), typeof(StunBehaviour));
        EnableBehaviours(typeof(EnemyBrain));
    }

    private void CancelArmor()
    {
        Active = false;
        onStop.Invoke();

        currentArmorHealth = 0;

        EnableBehaviours(typeof(KnockbackBehaviour), typeof(StunBehaviour));
        GetComponent<HittableBehaviour>().damageMultiplier /= damageMultiplier;
        StopBehaviours(typeof(BaseMovementBehaviour), typeof(ForcedBehaviour), typeof(AttackManager));
        DisableBehaviours(typeof(EnemyBrain));
        MovableObject.velocity = Vector3.zero;
    }

    public void HitArmor(float damage)
    {
        if (!Active) return;
        onHit.Invoke();
        currentArmorHealth = Mathf.Max(currentArmorHealth - damage, 0);
        if (currentArmorHealth == 0)
        {
            CancelArmor();
            onBreak.Invoke();
            armorCooldownStart = Time.time;
            reloadEvent = EventManager.StartTimeout(InitializeArmor, armorCooldown);
        }
    }

    public override void Stop()
    {
        EventManager.Instance.Detach(reloadEvent);
        CancelArmor();
    }

    public override float GetProgress()
    {
        return currentArmorHealth / armorHealth;
    }
}