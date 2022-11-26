using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Knockback,
    Stun
}

public class SimpleAttack : BaseAttack
{
    /// <value>
    /// This attack can be played only if the previous attack is one of the <c>previousAttacks</c>.
    /// If the attack can also be played without a previous attack, add <c>null</c> to the list.
    /// If the list is left empty, the attack can be played after any attack (including <c>null</c>).
    /// </value>
    public List<BaseAttack> previousAttacks;

    /// <value>
    /// If true, this attack can only play when <see cref="JumpBehaviour"/> is playing.
    /// </value>
    public bool midair;

    /// <value>
    /// Duration of the anticipation phase, in seconds.
    /// </value>
    public float anticipateDuration;

    /// <value>
    /// Duration of the active phase, in seconds.
    /// </value>
    public float activeDuration;

    /// <value>
    /// Duration of the recovery phase, in seconds.
    /// </value>
    public float recoveryDuration;

    /// <value>
    /// Hit detector that detect hits.
    /// </value>
    public BaseHitDetector hitDetector;

    /// <value>
    /// Health reduced to hit characters.
    /// </value>
    public float damage;

    /// <value>
    /// Side effect of the hit, either knockback or stun.
    /// </value>
    public HitType hitType = HitType.Knockback;

    /// <value>
    /// Power of the knockback, affects its initial speed.
    /// </value>
    public float knockbackPower;

    /// <value>
    /// Direction of the knockback in degrees, relative to X axis, in the direction of the hit. 
    /// </value>
    public float knockbackDirection;

    /// <value>
    /// Duration of stun effect caused by hit. If enemy is resistant to knockback, this value will be used too. 
    /// </value>
    public float stunTime = 0.5f;

    /// <value>
    /// An hittable was hit by this attack.
    /// </value>
    public event Action<IHittable> OnHit;

    public override void Awake()
    {
        base.Awake();
        OnPlay += StopOtherAttacks;
    }

    public void Start()
    {
        ConfigureHitDetector();
    }

    /// <summary>
    /// Disables walking when attack is playing, and re-enables it when attack is finished. 
    /// </summary>
    /// <param name="freeze">If <c>true</c>, changes the velocity to <see cref="Vector3.zero"/>.</param>
    protected void PreventWalking(bool freeze)
    {
        OnPlay += () =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            if (freeze)
            {
                MovableObject.velocity = Vector3.zero;
            }
        };
        generalEvents.onFinishRecovery.AddListener(() => EnableBehaviours(typeof(WalkBehaviour)));
        OnStop += () => EnableBehaviours(typeof(WalkBehaviour));
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

    /// <summary>
    /// Configures the <see cref="hitDetector"/>.
    /// The hit detector will detect hits when attack is during active phase.
    /// </summary>
    protected virtual void ConfigureHitDetector()
    {
        generalEvents.onStartActive.AddListener(() => hitDetector.StartDetector(HitCallable, AttackManager.hittableTags));
        generalEvents.onFinishActive.AddListener(() => hitDetector.StopDetector());
    }

    /// <summary>
    /// Tells if the attack can be played.
    /// Override to add more conditions that attack requires.
    /// </summary>
    /// <returns><c>true</c> if the attack can be played.</returns>
    public override bool CanPlay()
    {
        return base.CanPlay()
               && midair == IsPlaying(typeof(JumpBehaviour))
               && AllStopped(typeof(SlideBehaviour), typeof(DodgeBehaviour))
               && !((AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering) &&
                    !(instant && AttackManager.IsInterruptible()))
               && ComboCondition();
    }

    private bool ComboCondition()
    {
        return previousAttacks.Count == 0 || previousAttacks.Contains(AttackManager.lastAttack);
    }

    /// <summary>
    /// Calculates the attack damage on a specific hittable.
    /// </summary>
    /// <param name="character">Character to calculate the damage for.</param>
    /// <returns>Damage result.</returns>
    protected virtual float CalculateDamage(Character character)
    {
        return AttackManager.TranspileDamage(this, character, damage);
    }

    /// <summary>
    /// Invokes <see cref="OnHit"/> event
    /// </summary>
    /// <param name="hittable">Hittable hit by the attack</param>
    protected void InvokeOnHit(IHittable hittable)
    {
        OnHit?.Invoke(hittable);
    }

    /// <summary>
    /// Communicates with the hittable when hit by this attack.
    /// By default, Applies damage and performs knockback or stun.
    /// </summary>
    /// <param name="hittable">The hittable hit by the attack.</param>
    protected virtual void HitCallable(IHittable hittable)
    {
        InvokeOnHit(hittable);
        var processedDamage = CalculateDamage(hittable.Character);
        print(hittable.Character.name + " hit by " + AttackName);
        switch (hitType)
        {
            case HitType.Knockback:
                var hitDirection =
                    (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x - MovableObject.WorldPosition.x);
                hittable.Knockback(processedDamage, knockbackPower,
                    KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection), stunTime);
                break;
            case HitType.Stun:
                hittable.Stun(processedDamage, stunTime);
                break;
        }
    }

    /// <returns>Coroutine that waits for <see cref="anticipateDuration"/>.</returns>
    protected override IEnumerator AnticipateCoroutine()
    {
        yield return new WaitForSeconds(anticipateDuration);
    }

    /// <returns>Coroutine that waits for <see cref="activeDuration"/>.</returns>
    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitForSeconds(activeDuration);
    }

    /// <returns>Coroutine that waits for <see cref="recoveryDuration"/>.</returns>
    protected override IEnumerator RecoveryCoroutine()
    {
        yield return new WaitForSeconds(recoveryDuration);
    }
}