using UnityEngine;

/// <summary>
/// State machine parameters related to <see cref="KnockbackBehaviour"/>
/// </summary>
public class KnockbackBrainModule : BrainModule
{
    private const string KnockbackParameter = "knockback";
    private static readonly int Knockback = Animator.StringToHash(KnockbackParameter);

    private void Start()
    {
        var knockbackBehaviour = GetBehaviour<KnockbackBehaviour>();
        if (!knockbackBehaviour) return;
        knockbackBehaviour.PlayEvents.onPlay += () => StateMachine.SetBool(Knockback, true);
        knockbackBehaviour.PlayEvents.onStop += () => StateMachine.SetBool(Knockback, false);
    }

    public override string[] GetParameters()
    {
        return new[] { KnockbackParameter };
    }
}