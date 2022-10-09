using UnityEngine;
using static MathUtils;

[RequireComponent(typeof(Animator))]
public class GoblinBrain : CharacterBehaviour
{
    public string playerTag;
    public float rageHealthThreshold;

    private Animator stateMachine;

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<Animator>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        if (knockbackBehaviour)
        {
            knockbackBehaviour.onStart += () => stateMachine.SetBool("knockback", true);
            knockbackBehaviour.onRecover += () => stateMachine.SetBool("knockback", false);
        }
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        if (stunBehaviour)
        {
            stunBehaviour.onStart += () => stateMachine.SetBool("stun", true);
            stunBehaviour.onStop += () => stateMachine.SetBool("stun", false);
        }
    }

    public void Update()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem)
        {
            stateMachine.SetFloat("health", healthSystem.health);
            if (healthSystem.health <= rageHealthThreshold) {
                stateMachine.SetBool("rage", true);
            }
        }
        MovableObject player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<MovableObject>();
        if (player)
        {
            stateMachine.SetFloat("playerDistance", Vector2.Distance(ToPlane(movableObject.position), ToPlane(player.position)));
        }
    }
}
