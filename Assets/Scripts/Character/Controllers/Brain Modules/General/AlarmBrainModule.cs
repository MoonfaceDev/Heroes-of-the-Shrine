using UnityEngine;

public class AlarmBrainModule : BrainModule
{
    private const string AlarmParameter = "Alarm";
    private static readonly int Alarm = Animator.StringToHash(AlarmParameter);

    public void SetAlarm()
    {
        StateMachine.SetTrigger(Alarm);
    }

    public override string[] GetParameters()
    {
        return new[] { AlarmParameter };
    }
}