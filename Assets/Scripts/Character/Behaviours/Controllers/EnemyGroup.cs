using System.Linq;
using UnityEngine;

public class EnemyGroup : CharacterBehaviour
{
    private Animator stateMachine;
    private static readonly int EnemyCountParameter = Animator.StringToHash("enemyCount");
    private static readonly int ClosestEnemyDistanceParameter = Animator.StringToHash("closestEnemyDistance");
    private static readonly int IsClosestToPlayerParameter = Animator.StringToHash("isClosestToPlayer");
    private static readonly int EnemiesAttackingParameter = Animator.StringToHash("enemiesAttacking");

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<Animator>();
    }

    private void Update()
    {
        var enemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy")
            .Select(enemy => enemy.GetComponent<EnemyGroup>()).ToArray();

        // Enemy count
        stateMachine.SetInteger(EnemyCountParameter, enemies.Length);

        // Distance of the closest enemy
        if (enemies.Length > 1)
        {
            var closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                if (!prev) return next;
                if (!next || next == this)
                {
                    return prev;
                }

                var nextDistance = MovableObject.GroundDistance(next.GetComponent<MovableObject>().WorldPosition);
                var prevDistance = MovableObject.GroundDistance(prev.GetComponent<MovableObject>().WorldPosition);
                return nextDistance < prevDistance ? next : prev;
            }).GetComponent<MovableObject>();
            stateMachine.SetFloat(ClosestEnemyDistanceParameter,
                MovableObject.GroundDistance(closestEnemy.WorldPosition));
        }

        // Closest to the player
        var player = CachedObjectsManager.Instance.GetObject<Character>("Player");
        if (player)
        {
            var playerMovableObject = player.GetComponent<MovableObject>();
            var closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                if (!prev) return next;
                if (!next) return prev;
                var nextDistance = playerMovableObject.GroundDistance(next.GetComponent<MovableObject>().WorldPosition);
                var prevDistance = playerMovableObject.GroundDistance(prev.GetComponent<MovableObject>().WorldPosition);
                return nextDistance < prevDistance ? next : prev;
            });
            stateMachine.SetBool(IsClosestToPlayerParameter, this == closestEnemy);
        }

        // Enemies attacking
        var enemiesAttacking = enemies.Any(enemy =>
        {
            if (!enemy) return false;
            var enemyAttackManager = enemy.GetComponent<AttackManager>();
            return enemyAttackManager.Playing;
        });
        stateMachine.SetBool(EnemiesAttackingParameter, enemiesAttacking);
    }
}