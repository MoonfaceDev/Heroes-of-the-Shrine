using UnityEngine;
using UnityEngine.Events;

public class SuperArmorEffectCommand : ICommand
{
}

public class SuperArmorEffect : BaseEffect<SuperArmorEffectCommand>
{
    public float armorHealth;
    public float armorCooldown;
    public float damageMultiplier = 1;
    public UnityEvent onHit;
    public UnityEvent onBreak;

    private string reloadTimeout;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public void Start()
    {
        Play(new SuperArmorEffectCommand());
    }

    protected override void DoPlay(SuperArmorEffectCommand command)
    {
        Active = true;

        currentArmorHealth = armorHealth;

        GetComponent<HittableBehaviour>().damageMultiplier *= damageMultiplier;
        DisableBehaviours(typeof(StunBehaviour));
        StopBehaviours(typeof(StunBehaviour));
        EnableBehaviours(typeof(EnemyBrain));
    }

    public void HitArmor(float damage)
    {
        if (!Active) return;
        onHit.Invoke();
        currentArmorHealth = Mathf.Max(currentArmorHealth - damage, 0);
        if (currentArmorHealth == 0)
        {
            Stop();
            onBreak.Invoke();
            armorCooldownStart = Time.time;
            reloadTimeout = StartTimeout(() => Play(new SuperArmorEffectCommand()), armorCooldown);
        }
    }

    protected override void DoStop()
    {
        Cancel(reloadTimeout);
        Active = false;

        currentArmorHealth = 0;

        GetComponent<HittableBehaviour>().damageMultiplier /= damageMultiplier;
        EnableBehaviours(typeof(StunBehaviour));
        StopBehaviours(typeof(IMovementBehaviour), typeof(BaseAttack));
        DisableBehaviours(typeof(EnemyBrain));
        MovableObject.velocity = Vector3.zero;
    }

    public override float GetProgress()
    {
        return currentArmorHealth / armorHealth;
    }
}