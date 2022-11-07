using UnityEngine;
using static MathUtils;

[RequireComponent(typeof(Animator))]
public class EnemyBrain : CharacterController
{
    public string playerTag;
    public float rageHealthThreshold;
    public float rageDamageMultiplier = 1;

    private GameObject player;
    private MovableObject playerMovableObject;
    private Animator stateMachine;

    public void Alarm()
    {
        stateMachine.SetTrigger("Alarm");
    }

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<Animator>();
    }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
        {
            playerMovableObject = player.GetComponent<MovableObject>();
        }

        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        if (knockbackBehaviour)
        {
            knockbackBehaviour.OnPlay += () => stateMachine.SetBool("knockback", true);
            knockbackBehaviour.OnStop += () => stateMachine.SetBool("knockback", false);
        }

        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        if (stunBehaviour)
        {
            stunBehaviour.OnPlay += () => stateMachine.SetBool("stun", true);
            stunBehaviour.OnStop += () => stateMachine.SetBool("stun", false);
        }

        DieBehaviour dieBehaviour = GetComponent<DieBehaviour>();
        if (dieBehaviour)
        {
            dieBehaviour.OnDie += () => stateMachine.SetBool("dead", true);
        }

        AttackManager attackManager = GetComponent<AttackManager>();
        if (attackManager)
        {
            attackManager.AttachDamageMultiplier((BaseAttack attack, HittableBehaviour hittable) => IsEnraged() ? rageDamageMultiplier : 1);
        }

        foreach (BasePattern pattern in stateMachine.GetBehaviours<BasePattern>())
        {
            pattern.OnEnter += () => stateMachine.SetFloat("aggression", Random.Range(0f, 1f));
        }

        if (player)
        {
            KnockbackBehaviour playerKnockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
            if (knockbackBehaviour)
            {
                playerKnockbackBehaviour.OnPlay += OnPlayerKnockbackPlay;
                playerKnockbackBehaviour.OnFinish += OnPlayerKnockbackStop;
                playerKnockbackBehaviour.OnFinish += OnPlayerRecoveringFromKnockbackPlay;
                playerKnockbackBehaviour.OnRecover += OnPlayerRecoveringFromKnockbackStop;
            }

            StunBehaviour playerStunBehaviour = player.GetComponent<StunBehaviour>();
            if (playerStunBehaviour)
            {
                playerStunBehaviour.OnPlay += OnPlayerStunPlay;
                playerStunBehaviour.OnStop += OnPlayerStunStop;
            }

            AttackManager playerAttackManager = player.GetComponent<AttackManager>();
            if (playerAttackManager)
            {
                playerAttackManager.OnPlay += OnPlayerAttackPlay;
                playerAttackManager.OnStartAnticipating += OnPlayerAttackStartAnticipating;
                playerAttackManager.OnFinishAnticipating += OnPlayerAttackFinishAnticipating;
                playerAttackManager.OnStartActive += OnPlayerAttackStartActive;
                playerAttackManager.OnFinishActive += OnPlayerAttackFinishActive;
                playerAttackManager.OnStartRecovery += OnPlayerAttackStartRecovery;
                playerAttackManager.OnFinishRecovery += OnPlayerAttackFinishRecovery;
                playerAttackManager.OnStop += OnPlayerAttackStop;
            }
        }
    }

    private void OnPlayerKnockbackPlay() => stateMachine.SetBool("playerKnockback", true);
    private void OnPlayerKnockbackStop() => stateMachine.SetBool("playerKnockback", false);
    private void OnPlayerRecoveringFromKnockbackPlay() => stateMachine.SetBool("playerRecoveringFromKnockback", true);
    private void OnPlayerRecoveringFromKnockbackStop() => stateMachine.SetBool("playerRecoveringFromKnockback", false);
    private void OnPlayerStunPlay() => stateMachine.SetBool("playerStun", true);
    private void OnPlayerStunStop() => stateMachine.SetBool("playerStun", false);
    private void OnPlayerAttackPlay() => stateMachine.SetBool("playerAttacking", true);
    private void OnPlayerAttackStartAnticipating() => stateMachine.SetBool("playerAttacking-anticipating", true);
    private void OnPlayerAttackFinishAnticipating() => stateMachine.SetBool("playerAttacking-anticipating", false);
    private void OnPlayerAttackStartActive() => stateMachine.SetBool("playerAttacking-active", true);
    private void OnPlayerAttackFinishActive() => stateMachine.SetBool("playerAttacking-active", false);
    private void OnPlayerAttackStartRecovery() => stateMachine.SetBool("playerAttacking-recovering", true);
    private void OnPlayerAttackFinishRecovery() => stateMachine.SetBool("playerAttacking-recovering", false);
    private void OnPlayerAttackStop() => stateMachine.SetBool("playerAttacking", false);

    public void Update()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem)
        {
            stateMachine.SetFloat("health", healthSystem.health);
            if (IsEnraged()) {
                stateMachine.SetBool("rage", true);
            }
        }
        if (playerMovableObject)
        {
            stateMachine.SetFloat("playerDistance", Vector2.Distance(ToPlane(MovableObject.WorldPosition), ToPlane(playerMovableObject.WorldPosition)));
            stateMachine.SetFloat("playerDistanceX", Mathf.Abs((MovableObject.WorldPosition - playerMovableObject.WorldPosition).x));
            stateMachine.SetFloat("playerDistanceY", Mathf.Abs((MovableObject.WorldPosition - playerMovableObject.WorldPosition).y));
            stateMachine.SetFloat("playerDistanceZ", Mathf.Abs((MovableObject.WorldPosition - playerMovableObject.WorldPosition).z));
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
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        return healthSystem && healthSystem.health <= rageHealthThreshold;
    }

    public void StopBehaviours()
    {
        StopBehaviours(typeof(PlayableBehaviour));
    }

    private void OnDestroy()
    {
        if (player)
        {
            KnockbackBehaviour playerKnockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
            if (playerKnockbackBehaviour)
            {
                playerKnockbackBehaviour.OnPlay -= OnPlayerKnockbackPlay;
                playerKnockbackBehaviour.OnFinish -= OnPlayerKnockbackStop;
                playerKnockbackBehaviour.OnFinish -= OnPlayerRecoveringFromKnockbackPlay;
                playerKnockbackBehaviour.OnRecover -= OnPlayerRecoveringFromKnockbackStop;
            }

            StunBehaviour playerStunBehaviour = player.GetComponent<StunBehaviour>();
            if (playerStunBehaviour)
            {
                playerStunBehaviour.OnPlay -= OnPlayerStunPlay;
                playerStunBehaviour.OnStop -= OnPlayerStunStop;
            }

            AttackManager playerAttackManager = player.GetComponent<AttackManager>();
            if (playerAttackManager)
            {
                playerAttackManager.OnPlay -= OnPlayerAttackPlay;
                playerAttackManager.OnStartAnticipating -= OnPlayerAttackStartAnticipating;
                playerAttackManager.OnFinishAnticipating -= OnPlayerAttackFinishAnticipating;
                playerAttackManager.OnStartActive -= OnPlayerAttackStartActive;
                playerAttackManager.OnFinishActive -= OnPlayerAttackFinishActive;
                playerAttackManager.OnStartRecovery -= OnPlayerAttackStartRecovery;
                playerAttackManager.OnFinishRecovery -= OnPlayerAttackFinishRecovery;
                playerAttackManager.OnStop -= OnPlayerAttackStop;
            }
        }
    }
}
