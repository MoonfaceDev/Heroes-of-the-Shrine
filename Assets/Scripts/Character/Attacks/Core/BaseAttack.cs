using System;
using System.Collections.Generic;
using System.Linq;
using ExtEvents.OdinSerializer.Utilities;

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
public abstract class BaseAttack : PhasedBehaviour<BaseAttack.Command>
{
    public class Command
    {
    }

    /// <value>
    /// This attack can be played only if the previous attack is one of the <c>previousAttacks</c>.
    /// If the attack can also be played without a previous attack, add <c>null</c> to the list.
    /// If the list is left empty, the attack can be played after any attack (including <c>null</c>).
    /// </value>
    public List<BaseAttack> previousAttacks;

    /// <value>
    /// If true, the behaviour can be played while another interruptible behaviour is playing.
    /// </value>
    public bool instant;

    /// <value>
    /// If true, an instant behaviour can replace it while this behaviour is playing.
    /// </value>
    public bool interruptible = true;

    /// <value>
    /// If <c>true</c>, The behaviour cannot be interrupted while <see cref="PhasedBehaviour{T}.Recovering"/> is true.
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

    protected override void Awake()
    {
        base.Awake();
        if (!interruptible)
        {
            IPlayableBehaviour[] nonBlocked = { GetBehaviour<WalkBehaviour>() };
            var blockedBehaviours = GetBehaviours<IControlledBehaviour>().Except(nonBlocked).ToArray();
            var blockedAttacks = blockedBehaviours.Where(behaviour => behaviour is BaseAttack);

            PlayEvents.onPlay += () => blockedBehaviours.ForEach(behaviour => behaviour.Blocked = true);
            phaseEvents.onFinishActive += () => blockedAttacks.ForEach(behaviour => behaviour.Blocked = false);
            PlayEvents.onStop += () =>
                blockedBehaviours.Except(blockedAttacks).ForEach(behaviour => behaviour.Blocked = false);
        }
    }

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && ((!IsMidair && !IsPlaying<JumpBehaviour>()) || (IsMidair && AirCondition()))
               && (AttackManager.CanPlayAttack() || instant)
               && ComboCondition();
    }

    private bool AirCondition()
    {
        var jumpBehaviour = GetBehaviour<JumpBehaviour>();
        return jumpBehaviour && jumpBehaviour.Active;
    }

    private bool ComboCondition()
    {
        return previousAttacks.Count == 0 || previousAttacks.Contains(AttackManager.lastAttack);
    }

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
        base.DoPlay(command);
    }

    protected override void DoStop()
    {
        base.DoStop();
        if (Motion != MotionSettings.WalkingEnabled)
        {
            UnblockBehaviours(typeof(WalkBehaviour));
        }
    }

    protected void StartHitDetector(BaseHitDetector hitDetector, ChainHitExecutor hitExecutor)
    {
        hitDetector.StartDetector(hittable => hittable.Hit(hitExecutor,
            new Hit { source = this, victim = hittable, direction = Entity.WorldRotation }
        ), AttackManager.hittableTags);
    }
}