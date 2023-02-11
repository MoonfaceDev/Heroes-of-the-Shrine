using System.Collections.Generic;

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
    /// Motion setting
    /// </value>
    protected virtual MotionSettings Motion => MotionSettings.Static;

    /// <value>
    /// If <c>true</c>, this attack can only play when <see cref="JumpBehaviour"/> is playing
    /// </value>
    protected virtual bool IsMidair => false;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && ((!IsMidair && !IsPlaying<JumpBehaviour>()) || (IsMidair && AirCondition()))
               && AttackManager.CanPlayMove(instant)
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
}