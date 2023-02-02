using ExtEvents;
using UnityEngine;

public class SuperArmor : PlayableBehaviour<SuperArmor.Command>
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
    private static readonly int SuperArmorHash = Animator.StringToHash("SuperArmor");

    private float CurrentArmorHealth
    {
        get => currentArmorHealth;
        set
        {
            currentArmorHealth = value;
            Animator.SetBool(SuperArmorHash, value > 0);
        }
    }

    public override bool Playing => CurrentArmorHealth > 0;

    private void Start()
    {
        Play(new Command());
    }

    protected override void DoPlay(Command command)
    {
        CurrentArmorHealth = armorHealth;

        GetComponent<HittableBehaviour>().damageMultiplier *= damageMultiplier;
        BlockBehaviours(typeof(StunBehaviour));
        StopBehaviours(typeof(StunBehaviour));
        EnableComponents(typeof(BrainCore));
    }

    public void HitArmor(float damage)
    {
        if (!Playing) return;
        onHit.Invoke();
        CurrentArmorHealth = Mathf.Max(CurrentArmorHealth - damage, 0);
        if (CurrentArmorHealth == 0)
        {
            RemoveArmor();
            onBreak.Invoke();
            armorCooldownStart = Time.time;
            reloadTimeout = StartTimeout(() => Play(new Command()), armorCooldown);
        }
    }

    private void RemoveArmor()
    {
        Cancel(reloadTimeout);

        CurrentArmorHealth = 0;

        GetComponent<HittableBehaviour>().damageMultiplier /= damageMultiplier;
        UnblockBehaviours(typeof(StunBehaviour));
        StopBehaviours(typeof(IMovementBehaviour), typeof(BaseAttack));
        DisableComponents(typeof(BrainCore));
        MovableEntity.velocity = Vector3.zero;
    }

    protected override void DoStop()
    {
        RemoveArmor();
    }

    public float GetProgress()
    {
        return CurrentArmorHealth / armorHealth;
    }
}