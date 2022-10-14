using System.Linq;
using UnityEngine;

public class EnemyGroup : CharacterBehaviour
{
    private Animator stateMachine;

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<Animator>();
    }

    void Update()
    {
        EnemyGroup[] enemies = FindObjectsOfType<EnemyGroup>();

        // Enemy count
        stateMachine.SetInteger("enemyCount", enemies.Length);

        // Distance of the closest enemt
        if (enemies.Length > 1)
        {
            MovableObject closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                if (next == this)
                {
                    return prev;
                }
                float nextDistance = movableObject.GroundDistance(next.GetComponent<MovableObject>().position);
                float prevDistance = movableObject.GroundDistance(prev.GetComponent<MovableObject>().position);
                return nextDistance < prevDistance ? next : prev;
            }).GetComponent<MovableObject>();
            stateMachine.SetFloat("closestEnemyDistance", movableObject.GroundDistance(closestEnemy.position));
        }

        // Closest to the player
        MovableObject player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovableObject>();
        if (player)
        {
            EnemyGroup closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                float nextDistance = player.GroundDistance(next.GetComponent<MovableObject>().position);
                float prevDistance = player.GroundDistance(prev.GetComponent<MovableObject>().position);
                return nextDistance < prevDistance ? next : prev;
            });
            stateMachine.SetBool("isClosestToPlayer", this == closestEnemy);
        }

        // Are any enemies attacking
        SetEnemiesAttacking(enemies);
    }

    private void SetEnemiesAttacking(EnemyGroup[] enemies)
    {
        foreach (EnemyGroup enemy in enemies)
        {
            AttackManager enemyAttackManager = enemy.GetComponent<AttackManager>();
            if (enemyAttackManager && enemyAttackManager.attacking)
            {
                stateMachine.SetBool("enemiesAttacking", true);
                return;
            }
        }
        stateMachine.SetBool("enemiesAttacking", false);
    }
}
