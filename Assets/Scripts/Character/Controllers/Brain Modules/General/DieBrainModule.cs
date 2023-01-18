using UnityEngine;

public class DieBrainModule : BrainModule
{
    private const string DeadParameter = "dead";
    private static readonly int Dead = Animator.StringToHash(DeadParameter);

    public void Start()
    {
        var dieBehaviour = GetComponent<DieBehaviour>();
        if (!dieBehaviour) return;
        dieBehaviour.onDie.AddListener(() => StateMachine.SetBool(Dead, true));
    }

    public override string[] GetParameters()
    {
        return new[] { DeadParameter };
    }
}