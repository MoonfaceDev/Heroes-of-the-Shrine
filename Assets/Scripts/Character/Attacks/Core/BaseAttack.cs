using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Motion settings when playing attacks
/// </summary>
public enum MotionSettings
{
    WalkingEnabled, // Character can walk
    WalkingDisabled, // Character cannot walk, but it will keep moving in the same speed
    Static, // Character cannot walk, and stop right when attack is played
}

/// <summary>
/// Base class for all attacks. Most attacks should derive from <see cref="SimpleAttack"/>, which has more members and helper methods.
/// </summary>
public abstract class BaseAttack : PlayableBehaviour<BaseAttack.Command>, IControlledBehaviour
{
    public class Command
    {
    }

    /// <value>
    /// General attack events
    /// </value>
    public AttackEvents attackEvents;

    /// <value>
    /// This attack can be played only if the previous attack is one of the <c>previousAttacks</c>.
    /// If the attack can also be played without a previous attack, add <c>null</c> to the list.
    /// If the list is left empty, the attack can be played after any attack (including <c>null</c>).
    /// </value>
    public List<BaseAttack> previousAttacks;

    /// <value>
    /// If true, the attack can be played while another interruptible attack is playing.
    /// </value>
    public bool instant;

    /// <value>
    /// If true, an instant attack can replace it while this attack is playing.
    /// </value>
    public bool interruptible = true;

    /// <value>
    /// If <c>true</c>, The attack cannot be interrupted while <see cref="Recovering"/> is true.
    /// </value>
    public bool hardRecovery;

    /// <value>
    /// Motion setting
    /// </value>
    protected virtual MotionSettings Motion => MotionSettings.Static;

    /// <value>
    /// If <c>true</c>, this attack can only play when <see cref="JumpBehaviour"/> is playing
    /// </value>
    protected virtual bool IsMidair => false;

    /// <value>
    /// Attack is anticipating.
    /// It also sets the animator parameter: <c>{attackName}-anticipating</c>.
    /// </value>
    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(AttackName + "-anticipating", anticipating);
            (value ? attackEvents.onStartAnticipating : attackEvents.onFinishAnticipating).Invoke();
        }
    }

    /// <value>
    /// Attack is active.
    /// It also sets the animator parameter: <c>{attackName}-active</c>.
    /// </value>
    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(AttackName + "-active", active);
            (value ? attackEvents.onStartActive : attackEvents.onFinishActive).Invoke();
        }
    }

    /// <value>
    /// Attack is recovering.
    /// It also sets the animator parameter: <c>{attackName}-recovering</c>.
    /// </value>
    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(AttackName + "-recovering", recovering);
            (value ? attackEvents.onStartRecovery : attackEvents.onFinishRecovery).Invoke();
        }
    }

    public override bool Playing => Anticipating || Active || Recovering;

    private string AttackName => GetType().Name;
    private bool anticipating;
    private bool active;
    private bool recovering;
    private Coroutine attackFlowCoroutine;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && IsMidair == IsPlaying<JumpBehaviour>()
               && AttackManager.CanPlayMove(instant)
               && ComboCondition();
    }

    private bool ComboCondition()
    {
        return previousAttacks.Count == 0 || previousAttacks.Contains(AttackManager.lastAttack);
    }

    /// <summary>
    /// Play the attack phases
    /// </summary>
    protected override void DoPlay(Command command)
    {
        if (Motion != MotionSettings.WalkingEnabled)
        {
            var velocityBefore = MovableEntity.velocity;
            BlockBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            if (Motion != MotionSettings.Static)
            {
                MovableEntity.velocity = velocityBefore;
            }
        }

        StopBehaviours(typeof(BaseAttack));
        attackFlowCoroutine = StartCoroutine(AttackFlowCoroutine());
    }

    /// <summary>
    /// Anticipation phase coroutine
    /// </summary>
    protected abstract IEnumerator AnticipationPhase();
    
    /// <summary>
    /// Active phase coroutine
    /// </summary>
    protected abstract IEnumerator ActivePhase();
    
    /// <summary>
    /// Recovery phase coroutine
    /// </summary>
    protected abstract IEnumerator RecoveryPhase();

    private IEnumerator AttackFlowCoroutine()
    {
        Anticipating = true;
        yield return AnticipationPhase();
        Anticipating = false;
        Active = true;
        yield return ActivePhase();
        Active = false;
        Recovering = true;
        yield return RecoveryPhase();
        Stop();
    }

    /// <summary>
    /// Stops the attack immediately in any of its phases.
    /// Can be safely called even if attack is not currently playing, as it will do nothing.
    /// </summary>
    protected override void DoStop()
    {
        StopCoroutine(attackFlowCoroutine);

        if (Motion != MotionSettings.WalkingEnabled)
        {
            UnblockBehaviours(typeof(WalkBehaviour));
        }

        if (Anticipating)
        {
            Anticipating = false;
            attackEvents.onFinishAnticipating.Invoke();
        }

        if (Active)
        {
            Active = false;
            attackEvents.onFinishActive.Invoke();
        }

        if (Recovering)
        {
            Recovering = false;
            attackEvents.onFinishRecovery.Invoke();
        }
    }
}