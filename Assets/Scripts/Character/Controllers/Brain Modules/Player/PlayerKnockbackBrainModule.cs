using UnityEngine;

public class PlayerKnockbackBrainModule : BrainModule
{
    private const string PlayerKnockbackParameter = "playerKnockback";
    private static readonly int PlayerKnockback = Animator.StringToHash(PlayerKnockbackParameter);

    private const string PlayerRecoveringFromKnockbackParameter = "playerRecoveringFromKnockback";

    private static readonly int PlayerRecoveringFromKnockback =
        Animator.StringToHash(PlayerRecoveringFromKnockbackParameter);

    private KnockbackBehaviour knockbackBehaviour;

    public override void Awake()
    {
        base.Awake();

        var player = EntityManager.Instance.GetEntity(Tag.Player);
        knockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
        if (!knockbackBehaviour) return;

        knockbackBehaviour.PlayEvents.onPlay += OnPlay;
        knockbackBehaviour.OnFinish += OnStop;
        knockbackBehaviour.OnFinish += OnRecoveringPlay;
        knockbackBehaviour.OnRecover += OnRecoveringStop;
    }

    public override string[] GetParameters()
    {
        return new[] { PlayerKnockbackParameter, PlayerRecoveringFromKnockbackParameter };
    }

    private void OnDestroy()
    {
        if (!knockbackBehaviour) return;

        knockbackBehaviour.PlayEvents.onPlay -= OnPlay;
        knockbackBehaviour.OnFinish -= OnStop;
        knockbackBehaviour.OnFinish -= OnRecoveringPlay;
        knockbackBehaviour.OnRecover -= OnRecoveringStop;
    }

    private void OnPlay() => StateMachine.SetBool(PlayerKnockback, true);

    private void OnStop() => StateMachine.SetBool(PlayerKnockback, false);

    private void OnRecoveringPlay() =>
        StateMachine.SetBool(PlayerRecoveringFromKnockback, true);

    private void OnRecoveringStop() =>
        StateMachine.SetBool(PlayerRecoveringFromKnockback, false);
}