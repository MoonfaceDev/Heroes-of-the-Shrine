using UnityEngine;

/// <summary>
/// State machine trigger parameter that can be used for transition between non-aggressive state to aggressive state.
/// Must be called manually.
/// </summary>
public class AlarmBrainModule : BrainModule
{
    private const string AlarmParameter = "Alarm";
    private static readonly int Alarm = Animator.StringToHash(AlarmParameter);

    /// <summary>
    /// Sets the trigger
    /// </summary>
    public void SetAlarm()
    {
        StateMachine.SetTrigger(Alarm);
    }

    public override string[] GetParameters()
    {
        return new[] { AlarmParameter };
    }
}