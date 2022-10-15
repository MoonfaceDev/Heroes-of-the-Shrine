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

    public void Start()
    {
        EnemyGroup[] enemies = FindObjectsOfType<EnemyGroup>();
        int enemiesAttackingCount = 0;
        foreach (EnemyGroup enemy in enemies)
        {
            if (enemy != this)
            {
                AttackManager enemyAttackManager = enemy.GetComponent<AttackManager>();
                if (enemyAttackManager)
                {
                    enemyAttackManager.onAnticipate += () =>
                    {
                        enemiesAttackingCount++;
                        stateMachine.SetBool("enemiesAttacking", true);
                    };
                    enemyAttackManager.onStop += () =>
                    {
                        enemiesAttackingCount--;
                        if (enemiesAttackingCount == 0)
                        {
                            stateMachine.SetBool("enemiesAttacking", false);
                        }
                    };
                }
            }
        }
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
    }
}
