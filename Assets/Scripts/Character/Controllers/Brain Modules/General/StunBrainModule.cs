using UnityEngine;

public class StunBrainModule : BrainModule
{
    private const string StunParameter = "stun";
    private static readonly int Stun = Animator.StringToHash(StunParameter);

    public void Start()
    {
        var stunBehaviour = GetComponent<StunBehaviour>();
        if (!stunBehaviour) return;
        stunBehaviour.PlayEvents.onPlay += () => StateMachine.SetBool(Stun, true);
        stunBehaviour.PlayEvents.onStop += () => StateMachine.SetBool(Stun, false);
    }

    public override string[] GetParameters()
    {
        return new[] { StunParameter };
    }
}