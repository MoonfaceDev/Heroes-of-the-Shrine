using System.Collections.Generic;
using UnityEngine.Events;

public class SimpleHitExecutor : HitExecutor<HitDefinition>
{
    protected override HitDefinition HitDefinition { get; }

    public SimpleHitExecutor(HitDefinition hitDefinition)
    {
        HitDefinition = hitDefinition;
    }
}

public class SimpleAttack : BaseAttack
{
    /// <value>
    /// This attack can be played only if the previous attack is one of the <c>previousAttacks</c>.
    /// If the attack can also be played without a previous attack, add <c>null</c> to the list.
    /// If the list is left empty, the attack can be played after any attack (including <c>null</c>).
    /// </value>
    public List<BaseAttack> previousAttacks;

    public TimedAttackFlow attackFlow;

    /// <value>
    /// Hit detector
    /// </value>
    public BaseHitDetector hitDetector;

    /// <summary>
    /// Hit effect definition
    /// </summary>
    public HitDefinition hitDefinition;

    /// <value>
    /// An hittable was hit by this attack
    /// </value>
    public UnityEvent<IHittable> onHit;

    public override void Awake()
    {
        base.Awake();
        PlayEvents.onPlay.AddListener(StopOtherAttacks);
    }

    private void StopOtherAttacks()
    {
        var attackComponents = GetComponents<BaseAttack>();
        foreach (var attack in attackComponents)
        {
            if (attack != this)
            {
                attack.Stop();
            }
        }
    }

    public void Start()
    {
        ConfigureHitDetector();
    }

    /// <summary>
    /// Configures the <see cref="hitDetector"/>.
    /// The hit detector will detect hits when attack is during active phase.
    /// </summary>
    protected virtual void ConfigureHitDetector()
    {
        attackEvents.onStartActive.AddListener(() =>
            hitDetector.StartDetector(HitCallable, AttackManager.hittableTags));
        attackEvents.onFinishActive.AddListener(() => hitDetector.StopDetector());
    }

    /// <summary>
    /// Communicates with the hittable when hit by this attack.
    /// By default, Applies damage and performs knockback or stun.
    /// </summary>
    /// <param name="hittable">The hittable hit by the attack</param>
    protected virtual void HitCallable(IHittable hittable)
    {
        onHit.Invoke(hittable);
        new SimpleHitExecutor(hitDefinition).Execute(this, hittable);
    }

    /// <summary>
    /// Disables walking when attack is playing, and re-enables it when attack is finished 
    /// </summary>
    /// <param name="keepSpeed">If <c>true</c>, keeps speed from before</param>
    protected void PreventWalking(bool keepSpeed = false)
    {
        PlayEvents.onPlay.AddListener(() =>
        {
            var velocityBefore = MovableObject.velocity;
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            if (keepSpeed)
            {
                MovableObject.velocity = velocityBefore;
            }
        });
        PlayEvents.onStop.AddListener(() => EnableBehaviours(typeof(WalkBehaviour)));
    }

    protected override IAttackFlow AttackFlow => attackFlow;

    public override bool CanPlay(BaseAttackCommand command)
    {
        return base.CanPlay(command)
               && IsMidair == IsPlaying<JumpBehaviour>()
               && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && !((AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering) &&
                    !(instant && AttackManager.IsInterruptible()))
               && ComboCondition();
    }

    /// <value>
    /// If <c>true</c>, this attack can only play when <see cref="JumpBehaviour"/> is playing
    /// </value>
    protected virtual bool IsMidair => false;

    private bool ComboCondition()
    {
        return previousAttacks.Count == 0 || previousAttacks.Contains(AttackManager.lastAttack);
    }
}