using ExtEvents;
using UnityEngine;

public class SuperArmorEffect : BaseEffect<SuperArmorEffect.Command>
{
    public class Command
    {
    }

    public float armorHealth;
    public float armorCooldown;
    public float damageMultiplier = 1;
    [SerializeField] public ExtEvent onHit;
    [SerializeField] public ExtEvent onBreak;

    private string reloadTimeout;

    [HideInInspector] public float armorCooldownStart;
    [ShowDebug] private float currentArmorHealth;

    public void Start()
    {
        Play(new Command());
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentArmorHealth = armorHealth;

        GetComponent<HittableBehaviour>().damageMultiplier *= damageMultiplier;
        DisableBehaviours(typeof(StunBehaviour));
        StopBehaviours(typeof(StunBehaviour));
        EnableBehaviours(typeof(BrainCore));
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
            reloadTimeout = StartTimeout(() => Play(new Command()), armorCooldown);
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
        DisableBehaviours(typeof(BrainCore));
        MovableEntity.velocity = Vector3.zero;
    }

    public override float GetProgress()
    {
        return currentArmorHealth / armorHealth;
    }
}