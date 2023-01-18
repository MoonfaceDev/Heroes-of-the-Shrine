using UnityEngine;

public class PlayerAttackBrainModule : BrainModule
{
    private AttackManager attackManager;

    private static readonly int PlayerAttacking = Animator.StringToHash(PlayerAttackingParameter);

    private static readonly int PlayerAttackingAnticipating =
        Animator.StringToHash(PlayerAttackingAnticipatingParameter);

    private static readonly int PlayerAttackingActive = Animator.StringToHash(PlayerAttackingActiveParameter);

    private static readonly int PlayerAttackingRecovering = Animator.StringToHash(PlayerAttackingRecoveringParameter);

    private const string PlayerAttackingParameter = "playerAttacking";
    private const string PlayerAttackingAnticipatingParameter = "playerAttacking-anticipating";
    private const string PlayerAttackingActiveParameter = "playerAttacking-active";
    private const string PlayerAttackingRecoveringParameter = "playerAttacking-recovering";

    public override void Awake()
    {
        base.Awake();
        var player = EntityManager.Instance.GetEntity(Tag.Player);
        attackManager = player.GetComponent<AttackManager>();
        if (!attackManager) return;

        attackManager.PlayEvents.onPlay.AddListener(OnPlay);
        attackManager.attackEvents.onStartAnticipating.AddListener(OnStartAnticipating);
        attackManager.attackEvents.onFinishAnticipating.AddListener(OnFinishAnticipating);
        attackManager.attackEvents.onStartActive.AddListener(OnStartActive);
        attackManager.attackEvents.onFinishActive.AddListener(OnFinishActive);
        attackManager.attackEvents.onStartRecovery.AddListener(OnStartRecovery);
        attackManager.attackEvents.onFinishRecovery.AddListener(OnFinishRecovery);
        attackManager.PlayEvents.onStop.AddListener(OnStop);
    }

    private void OnDestroy()
    {
        if (!attackManager) return;

        attackManager.PlayEvents.onPlay.RemoveListener(OnPlay);
        attackManager.attackEvents.onStartAnticipating.RemoveListener(OnStartAnticipating);
        attackManager.attackEvents.onFinishAnticipating.RemoveListener(OnFinishAnticipating);
        attackManager.attackEvents.onStartActive.RemoveListener(OnStartActive);
        attackManager.attackEvents.onFinishActive.RemoveListener(OnFinishActive);
        attackManager.attackEvents.onStartRecovery.RemoveListener(OnStartRecovery);
        attackManager.attackEvents.onFinishRecovery.RemoveListener(OnFinishRecovery);
        attackManager.PlayEvents.onStop.RemoveListener(OnStop);
    }

    private void OnPlay() => StateMachine.SetBool(PlayerAttacking, true);
    private void OnStartAnticipating() => StateMachine.SetBool(PlayerAttackingAnticipating, true);

    private void OnFinishAnticipating() =>
        StateMachine.SetBool(PlayerAttackingAnticipating, false);

    private void OnStartActive() => StateMachine.SetBool(PlayerAttackingActive, true);
    private void OnFinishActive() => StateMachine.SetBool(PlayerAttackingActive, false);
    private void OnStartRecovery() => StateMachine.SetBool(PlayerAttackingRecovering, true);
    private void OnFinishRecovery() => StateMachine.SetBool(PlayerAttackingRecovering, false);
    private void OnStop() => StateMachine.SetBool(PlayerAttacking, false);

    public override string[] GetParameters()
    {
        return new[]
        {
            PlayerAttackingParameter, PlayerAttackingAnticipatingParameter, PlayerAttackingActiveParameter,
            PlayerAttackingRecoveringParameter
        };
    }
}