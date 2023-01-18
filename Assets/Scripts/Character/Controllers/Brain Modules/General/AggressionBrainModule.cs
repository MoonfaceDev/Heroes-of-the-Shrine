using UnityEngine;

public class AggressionBrainModule : BrainModule
{
    private const string AggressionParameter = "aggression";
    private static readonly int Aggression = Animator.StringToHash(AggressionParameter);

    public override void Awake()
    {
        base.Awake();
        foreach (var pattern in StateMachine.GetBehaviours<BasePattern>())
        {
            pattern.OnEnter += () => StateMachine.SetFloat(Aggression, Random.Range(0f, 1f));
        }
    }

    public override string[] GetParameters()
    {
        return new[] { AggressionParameter };
    }
}