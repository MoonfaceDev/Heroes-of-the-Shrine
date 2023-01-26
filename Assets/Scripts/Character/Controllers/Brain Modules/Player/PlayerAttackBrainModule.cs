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

        attackManager.PlayEvents.onPlay += OnPlay;
        attackManager.attackEvents.onStartAnticipating += OnStartAnticipating;
        attackManager.attackEvents.onFinishAnticipating += OnFinishAnticipating;
        attackManager.attackEvents.onStartActive += OnStartActive;
        attackManager.attackEvents.onFinishActive += OnFinishActive;
        attackManager.attackEvents.onStartRecovery += OnStartRecovery;
        attackManager.attackEvents.onFinishRecovery += OnFinishRecovery;
        attackManager.PlayEvents.onStop += OnStop;
    }

    private void OnDestroy()
    {
        if (!attackManager) return;

        attackManager.PlayEvents.onPlay -= OnPlay;
        attackManager.attackEvents.onStartAnticipating -= OnStartAnticipating;
        attackManager.attackEvents.onFinishAnticipating -= OnFinishAnticipating;
        attackManager.attackEvents.onStartActive -= OnStartActive;
        attackManager.attackEvents.onFinishActive -= OnFinishActive;
        attackManager.attackEvents.onStartRecovery -= OnStartRecovery;
        attackManager.attackEvents.onFinishRecovery -= OnFinishRecovery;
        attackManager.PlayEvents.onStop -= OnStop;
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