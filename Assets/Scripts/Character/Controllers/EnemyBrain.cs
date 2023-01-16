using System;
using System.Collections.Generic;
using UnityEngine;
using static MathUtils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class EnemyBrain : CharacterController
{
    public float rageHealthThreshold;
    public float rageDamageMultiplier = 1;

    [Serializable]
    public class DistanceParameterEntry
    {
        public string parameterName;
        public Vector2 groundPoint;
    }

    /// <value>
    /// Mapping from animator parameter name, to the point (on the ground) from which distance is measured.
    /// </value>
    public List<DistanceParameterEntry> distanceAnimatorParameters;

    private GameEntity player;
    private MovableEntity playerMovableEntity;
    protected Animator stateMachine;

    private static readonly int AlarmParameter = Animator.StringToHash("Alarm");
    private static readonly int KnockbackParameter = Animator.StringToHash("knockback");
    private static readonly int StunParameter = Animator.StringToHash("stun");
    private static readonly int DeadParameter = Animator.StringToHash("dead");
    private static readonly int AggressionParameter = Animator.StringToHash("aggression");
    private static readonly int PlayerKnockbackParameter = Animator.StringToHash("playerKnockback");

    private static readonly int PlayerRecoveringFromKnockbackParameter =
        Animator.StringToHash("playerRecoveringFromKnockback");

    private static readonly int PlayerStunParameter = Animator.StringToHash("playerStun");
    private static readonly int PlayerAttackingParameter = Animator.StringToHash("playerAttacking");

    private static readonly int PlayerAttackingAnticipatingParameter =
        Animator.StringToHash("playerAttacking-anticipating");

    private static readonly int PlayerAttackingActiveParameter = Animator.StringToHash("playerAttacking-active");

    private static readonly int PlayerAttackingRecoveringParameter =
        Animator.StringToHash("playerAttacking-recovering");

    private static readonly int HealthParameter = Animator.StringToHash("health");
    private static readonly int RageParameter = Animator.StringToHash("rage");
    private static readonly int PlayerDistanceParameter = Animator.StringToHash("playerDistance");
    private static readonly int PlayerDistanceXParameter = Animator.StringToHash("playerDistanceX");
    private static readonly int PlayerDistanceYParameter = Animator.StringToHash("playerDistanceY");
    private static readonly int PlayerDistanceZParameter = Animator.StringToHash("playerDistanceZ");

    public void Alarm()
    {
        stateMachine.SetTrigger(AlarmParameter);
    }

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        player = EntityManager.Instance.GetEntity(Tag.Player);
        if (player)
        {
            playerMovableEntity = player.GetComponent<MovableEntity>();
        }

        var knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        if (knockbackBehaviour)
        {
            knockbackBehaviour.PlayEvents.onPlay.AddListener(() => stateMachine.SetBool(KnockbackParameter, true));
            knockbackBehaviour.PlayEvents.onStop.AddListener(() => stateMachine.SetBool(KnockbackParameter, false));
        }

        var stunBehaviour = GetComponent<StunBehaviour>();
        if (stunBehaviour)
        {
            stunBehaviour.PlayEvents.onPlay.AddListener(() => stateMachine.SetBool(StunParameter, true));
            stunBehaviour.PlayEvents.onStop.AddListener(() => stateMachine.SetBool(StunParameter, false));
        }

        var dieBehaviour = GetComponent<DieBehaviour>();
        if (dieBehaviour)
        {
            dieBehaviour.onDie.AddListener(() => stateMachine.SetBool(DeadParameter, true));
        }

        var attackManager = GetComponent<AttackManager>();
        if (attackManager)
        {
            attackManager.DamageTranspiler.AttachDamageMultiplier((_, _) => IsEnraged() ? rageDamageMultiplier : 1);
        }

        foreach (var pattern in stateMachine.GetBehaviours<BasePattern>())
        {
            pattern.OnEnter += () => stateMachine.SetFloat(AggressionParameter, Random.Range(0f, 1f));
        }

        if (player)
        {
            var playerKnockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
            if (knockbackBehaviour)
            {
                playerKnockbackBehaviour.PlayEvents.onPlay.AddListener(OnPlayerKnockbackPlay);
                playerKnockbackBehaviour.OnFinish += OnPlayerKnockbackStop;
                playerKnockbackBehaviour.OnFinish += OnPlayerRecoveringFromKnockbackPlay;
                playerKnockbackBehaviour.OnRecover += OnPlayerRecoveringFromKnockbackStop;
            }

            var playerStunBehaviour = player.GetComponent<StunBehaviour>();
            if (playerStunBehaviour)
            {
                playerStunBehaviour.PlayEvents.onPlay.AddListener(OnPlayerStunPlay);
                playerStunBehaviour.PlayEvents.onStop.AddListener(OnPlayerStunStop);
            }

            var playerAttackManager = player.GetComponent<AttackManager>();
            if (playerAttackManager)
            {
                playerAttackManager.PlayEvents.onPlay.AddListener(OnPlayerAttackPlay);
                playerAttackManager.attackEvents.onStartAnticipating.AddListener(OnPlayerAttackStartAnticipating);
                playerAttackManager.attackEvents.onFinishAnticipating.AddListener(OnPlayerAttackFinishAnticipating);
                playerAttackManager.attackEvents.onStartActive.AddListener(OnPlayerAttackStartActive);
                playerAttackManager.attackEvents.onFinishActive.AddListener(OnPlayerAttackFinishActive);
                playerAttackManager.attackEvents.onStartRecovery.AddListener(OnPlayerAttackStartRecovery);
                playerAttackManager.attackEvents.onFinishRecovery.AddListener(OnPlayerAttackFinishRecovery);
                playerAttackManager.PlayEvents.onStop.AddListener(OnPlayerAttackStop);
            }
        }
    }

    private void OnPlayerKnockbackPlay() => stateMachine.SetBool(PlayerKnockbackParameter, true);
    private void OnPlayerKnockbackStop() => stateMachine.SetBool(PlayerKnockbackParameter, false);

    private void OnPlayerRecoveringFromKnockbackPlay() =>
        stateMachine.SetBool(PlayerRecoveringFromKnockbackParameter, true);

    private void OnPlayerRecoveringFromKnockbackStop() =>
        stateMachine.SetBool(PlayerRecoveringFromKnockbackParameter, false);

    private void OnPlayerStunPlay() => stateMachine.SetBool(PlayerStunParameter, true);
    private void OnPlayerStunStop() => stateMachine.SetBool(PlayerStunParameter, false);
    private void OnPlayerAttackPlay() => stateMachine.SetBool(PlayerAttackingParameter, true);
    private void OnPlayerAttackStartAnticipating() => stateMachine.SetBool(PlayerAttackingAnticipatingParameter, true);

    private void OnPlayerAttackFinishAnticipating() =>
        stateMachine.SetBool(PlayerAttackingAnticipatingParameter, false);

    private void OnPlayerAttackStartActive() => stateMachine.SetBool(PlayerAttackingActiveParameter, true);
    private void OnPlayerAttackFinishActive() => stateMachine.SetBool(PlayerAttackingActiveParameter, false);
    private void OnPlayerAttackStartRecovery() => stateMachine.SetBool(PlayerAttackingRecoveringParameter, true);
    private void OnPlayerAttackFinishRecovery() => stateMachine.SetBool(PlayerAttackingRecoveringParameter, false);
    private void OnPlayerAttackStop() => stateMachine.SetBool(PlayerAttackingParameter, false);

    protected override void Update()
    {
        base.Update();
        var healthSystem = GetComponent<HealthSystem>();
        if (healthSystem)
        {
            stateMachine.SetFloat(HealthParameter, healthSystem.health);
            if (IsEnraged())
            {
                stateMachine.SetBool(RageParameter, true);
            }
        }

        if (playerMovableEntity)
        {
            stateMachine.SetFloat(PlayerDistanceParameter,
                Vector2.Distance(ToPlane(MovableEntity.WorldPosition), ToPlane(playerMovableEntity.WorldPosition)));
            stateMachine.SetFloat(PlayerDistanceXParameter,
                Mathf.Abs((MovableEntity.WorldPosition - playerMovableEntity.WorldPosition).x));
            stateMachine.SetFloat(PlayerDistanceYParameter,
                Mathf.Abs((MovableEntity.WorldPosition - playerMovableEntity.WorldPosition).y));
            stateMachine.SetFloat(PlayerDistanceZParameter,
                Mathf.Abs((MovableEntity.WorldPosition - playerMovableEntity.WorldPosition).z));
        }

        foreach (var entry in distanceAnimatorParameters)
        {
            stateMachine.SetFloat(entry.parameterName,
                Vector2.Distance(ToPlane(MovableEntity.WorldPosition), entry.groundPoint));
        }
    }

    private void OnEnable()
    {
        stateMachine.enabled = true;
    }

    private void OnDisable()
    {
        stateMachine.enabled = false;
    }

    private bool IsEnraged()
    {
        var healthSystem = GetComponent<HealthSystem>();
        return healthSystem && healthSystem.health <= rageHealthThreshold;
    }

    private void OnDestroy()
    {
        if (player)
        {
            var playerKnockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
            if (playerKnockbackBehaviour)
            {
                playerKnockbackBehaviour.PlayEvents.onPlay.RemoveListener(OnPlayerKnockbackPlay);
                playerKnockbackBehaviour.OnFinish -= OnPlayerKnockbackStop;
                playerKnockbackBehaviour.OnFinish -= OnPlayerRecoveringFromKnockbackPlay;
                playerKnockbackBehaviour.OnRecover -= OnPlayerRecoveringFromKnockbackStop;
            }

            var playerStunBehaviour = player.GetComponent<StunBehaviour>();
            if (playerStunBehaviour)
            {
                playerStunBehaviour.PlayEvents.onPlay.RemoveListener(OnPlayerStunPlay);
                playerStunBehaviour.PlayEvents.onStop.RemoveListener(OnPlayerStunStop);
            }

            var playerAttackManager = player.GetComponent<AttackManager>();
            if (playerAttackManager)
            {
                playerAttackManager.PlayEvents.onPlay.RemoveListener(OnPlayerAttackPlay);
                playerAttackManager.attackEvents.onStartAnticipating.RemoveListener(OnPlayerAttackStartAnticipating);
                playerAttackManager.attackEvents.onFinishAnticipating.RemoveListener(OnPlayerAttackFinishAnticipating);
                playerAttackManager.attackEvents.onStartActive.RemoveListener(OnPlayerAttackStartActive);
                playerAttackManager.attackEvents.onFinishActive.RemoveListener(OnPlayerAttackFinishActive);
                playerAttackManager.attackEvents.onStartRecovery.RemoveListener(OnPlayerAttackStartRecovery);
                playerAttackManager.attackEvents.onFinishRecovery.RemoveListener(OnPlayerAttackFinishRecovery);
                playerAttackManager.PlayEvents.onStop.RemoveListener(OnPlayerAttackStop);
            }
        }
    }
}