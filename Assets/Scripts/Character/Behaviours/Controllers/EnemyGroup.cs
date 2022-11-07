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
        EnemyGroup[] enemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy").Select(enemy => enemy.GetComponent<EnemyGroup>()).ToArray();

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
                float nextDistance = MovableObject.GroundDistance(next.GetComponent<MovableObject>().WorldPosition);
                float prevDistance = MovableObject.GroundDistance(prev.GetComponent<MovableObject>().WorldPosition);
                return nextDistance < prevDistance ? next : prev;
            }).GetComponent<MovableObject>();
            stateMachine.SetFloat("closestEnemyDistance", MovableObject.GroundDistance(closestEnemy.WorldPosition));
        }

        // Closest to the player
        Character player = CachedObjectsManager.Instance.GetObject<Character>("Player");
        if (player)
        {
            MovableObject playerMovableObject = player.GetComponent<MovableObject>();
            EnemyGroup closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                float nextDistance = playerMovableObject.GroundDistance(next.GetComponent<MovableObject>().WorldPosition);
                float prevDistance = playerMovableObject.GroundDistance(prev.GetComponent<MovableObject>().WorldPosition);
                return nextDistance < prevDistance ? next : prev;
            });
            stateMachine.SetBool("isClosestToPlayer", this == closestEnemy);
        }

        // Enemies attacking
        bool enemiesAttacking = enemies.Any((enemy) =>
        {
            AttackManager enemyAttackManager = enemy.GetComponent<AttackManager>();
            return enemyAttackManager.Playing;
        });
        stateMachine.SetBool("enemiesAttacking", enemiesAttacking);
    }
}
