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

    protected override void Update()
    {
        base.Update();
        var enemies = EntityManager.Instance.GetEntities(Tag.Enemy)
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

                var nextDistance = MovableEntity.GroundDistance(next.GetComponent<MovableEntity>().WorldPosition);
                var prevDistance = MovableEntity.GroundDistance(prev.GetComponent<MovableEntity>().WorldPosition);
                return nextDistance < prevDistance ? next : prev;
            }).GetComponent<MovableEntity>();
            stateMachine.SetFloat(ClosestEnemyDistanceParameter,
                MovableEntity.GroundDistance(closestEnemy.WorldPosition));
        }

        // Closest to the player
        var player = EntityManager.Instance.GetEntity(Tag.Player);
        if (player)
        {
            var playerMovableObject = player.GetComponent<MovableEntity>();
            var closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
            {
                if (!prev) return next;
                if (!next) return prev;
                var nextDistance = playerMovableObject.GroundDistance(next.GetComponent<MovableEntity>().WorldPosition);
                var prevDistance = playerMovableObject.GroundDistance(prev.GetComponent<MovableEntity>().WorldPosition);
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