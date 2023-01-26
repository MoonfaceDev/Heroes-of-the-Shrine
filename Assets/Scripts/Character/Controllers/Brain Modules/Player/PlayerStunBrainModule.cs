using UnityEngine;

public class PlayerStunBrainModule : BrainModule
{
    private StunBehaviour stunBehaviour;

    private const string PlayerStunParameter = "playerStun";
    private static readonly int PlayerStun = Animator.StringToHash(PlayerStunParameter);

    public override void Awake()
    {
        base.Awake();

        var player = EntityManager.Instance.GetEntity(Tag.Player);
        stunBehaviour = player.GetComponent<StunBehaviour>();
        if (!stunBehaviour) return;

        stunBehaviour.PlayEvents.onPlay += OnPlayerStunPlay;
        stunBehaviour.PlayEvents.onStop += OnPlayerStunStop;
    }

    public override string[] GetParameters()
    {
        return new[] { PlayerStunParameter };
    }

    private void OnDestroy()
    {
        if (!stunBehaviour) return;

        stunBehaviour.PlayEvents.onPlay -= OnPlayerStunPlay;
        stunBehaviour.PlayEvents.onStop -= OnPlayerStunStop;
    }

    private void OnPlayerStunPlay() => StateMachine.SetBool(PlayerStun, true);
    private void OnPlayerStunStop() => StateMachine.SetBool(PlayerStun, false);
}