using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="StunBehaviour"/>
/// </summary>
public class StunBrainModule : BrainModule
{
    private const string StunParameter = "stun";
    private static readonly int Stun = Animator.StringToHash(StunParameter);

    private void Start()
    {
        var stunBehaviour = GetBehaviour<StunBehaviour>();
        if (!stunBehaviour) return;
        stunBehaviour.PlayEvents.onPlay += () => StateMachine.SetBool(Stun, true);
        stunBehaviour.PlayEvents.onStop += () => StateMachine.SetBool(Stun, false);
    }

    public override string[] GetParameters()
    {
        return new[] { StunParameter };
    }
}