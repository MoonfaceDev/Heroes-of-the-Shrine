using UnityEngine;

/// <summary>
/// State machine aggression parameter, assigned with a random value (0-1) before each state transition.
/// Can be used for randomizing transitions.
/// </summary>
public class AggressionBrainModule : BrainModule
{
    private const string AggressionParameter = "aggression";
    private static readonly int Aggression = Animator.StringToHash(AggressionParameter);

    private void Start()
    {
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