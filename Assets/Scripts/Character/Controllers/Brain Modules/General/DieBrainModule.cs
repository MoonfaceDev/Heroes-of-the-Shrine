using System.Linq;
using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="DieBehaviour"/>
/// </summary>
public class DieBrainModule : BrainModule
{
    private const string DeadParameter = "dead";
    private static readonly int Dead = Animator.StringToHash(DeadParameter);

    [InjectBehaviour] private DieBehaviour dieBehaviour;

    private void Start()
    {
        dieBehaviour.onDie += () =>
        {
            if (!HasParameter())
            {
                Debug.LogError("Parameter \"Dead\" not found! It should be used to exit any other state when " +
                               "character dies");
            }

            StateMachine.SetBool(Dead, true);
        };
    }

    private bool HasParameter()
    {
        return StateMachine.parameters.Any(param => param.name == DeadParameter);
    }

    public override string[] GetParameters()
    {
        return new[] { DeadParameter };
    }
}