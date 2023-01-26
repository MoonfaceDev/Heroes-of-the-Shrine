using UnityEngine;

public class KnockbackBrainModule : BrainModule
{
    private const string KnockbackParameter = "knockback";
    private static readonly int Knockback = Animator.StringToHash(KnockbackParameter);

    public void Start()
    {
        var knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        if (!knockbackBehaviour) return;
        knockbackBehaviour.PlayEvents.onPlay += () => StateMachine.SetBool(Knockback, true);
        knockbackBehaviour.PlayEvents.onStop += () => StateMachine.SetBool(Knockback, false);
    }

    public override string[] GetParameters()
    {
        return new[] { KnockbackParameter };
    }
}