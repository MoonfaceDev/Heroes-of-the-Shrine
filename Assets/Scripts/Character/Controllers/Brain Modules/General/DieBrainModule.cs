using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="DieBehaviour"/>
/// </summary>
public class DieBrainModule : BrainModule
{
    private const string DeadParameter = "dead";
    private static readonly int Dead = Animator.StringToHash(DeadParameter);

    private void Start()
    {
        var dieBehaviour = GetComponent<DieBehaviour>();
        if (!dieBehaviour) return;
        dieBehaviour.onDie += () => StateMachine.SetBool(Dead, true);
    }

    public override string[] GetParameters()
    {
        return new[] { DeadParameter };
    }
}