using UnityEngine;
using static MathUtils;

[RequireComponent(typeof(Animator))]
public class EnemyBrain : CharacterBehaviour
{
    public string playerTag;
    public float rageHealthThreshold;
    public float rageDamageMultiplier = 1;

    private Animator stateMachine;

    public void Start()
    {
        stateMachine = GetComponent<Animator>();

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

        AttackManager attackManager = GetComponent<AttackManager>();
        if (attackManager)
        {
            attackManager.AttachDamageMultiplier((BaseAttack attack, HittableBehaviour hittable) => IsEnraged() ? rageDamageMultiplier : 1);
        }

        foreach (BasePattern pattern in stateMachine.GetBehaviours<BasePattern>())
        {
            pattern.onEnter += () => stateMachine.SetFloat("aggression", Random.Range(0f, 1f));
        }

        MovableObject player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<MovableObject>();
        if (player)
        {
            KnockbackBehaviour playerKnockbackBehaviour = player.GetComponent<KnockbackBehaviour>();
            playerKnockbackBehaviour.OnPlay += () => stateMachine.SetBool("playerKnockback", true);
            playerKnockbackBehaviour.OnFinish += () => stateMachine.SetBool("playerKnockback", false);
            playerKnockbackBehaviour.OnFinish += () => stateMachine.SetBool("playerRecoveringFromKnockback", true);
            playerKnockbackBehaviour.OnRecover += () => stateMachine.SetBool("playerRecoveringFromKnockback", false);

            StunBehaviour playerStunBehaviour = player.GetComponent<StunBehaviour>();
            playerStunBehaviour.OnPlay += () => stateMachine.SetBool("playerStun", true);
            playerStunBehaviour.OnStop += () => stateMachine.SetBool("playerStun", false);

            AttackManager playerAttackManager = player.GetComponent<AttackManager>();
            playerAttackManager.OnPlay += () => stateMachine.SetBool("playerAttacking", true);
            playerAttackManager.OnStop += () => stateMachine.SetBool("playerAttacking", false);
        }
    }

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
        MovableObject player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<MovableObject>();
        if (player)
        {
            stateMachine.SetFloat("playerDistance", Vector2.Distance(ToPlane(MovableObject.position), ToPlane(player.position)));
        }
    }

    private bool IsEnraged()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        return healthSystem && healthSystem.health <= rageHealthThreshold;
    }
}
