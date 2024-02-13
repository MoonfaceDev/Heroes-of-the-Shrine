using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="StunBehaviour"/>
/// </summary>
public class StunBrainModule : BrainModule
{
    [InjectBehaviour] private StunBehaviour stunBehaviour;
    
    private const string StunParameter = "stun";
    private static readonly int Stun = Animator.StringToHash(StunParameter);

    private void Start()
    {
        stunBehaviour.PlayEvents.onPlay += () => StateMachine.SetBool(Stun, true);
        stunBehaviour.PlayEvents.onStop += () => StateMachine.SetBool(Stun, false);
    }

    public override string[] GetParameters()
    {
        return new[] { StunParameter };
    }
}