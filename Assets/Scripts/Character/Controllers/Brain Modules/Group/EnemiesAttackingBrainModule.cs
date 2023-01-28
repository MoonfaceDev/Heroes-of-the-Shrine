using System.Linq;
using UnityEngine;

/// <summary>
/// State machine parameter telling if any enemy is attacking, including itself
/// </summary>
public class EnemiesAttackingBrainModule : BrainModule
{
    private const string EnemiesAttackingParameter = "enemiesAttacking";
    private static readonly int EnemiesAttacking = Animator.StringToHash(EnemiesAttackingParameter);

    protected override void Update()
    {
        base.Update();

        var enemies = EntityManager.Instance.GetEntities(Tag.Enemy);

        var enemiesAttacking = enemies.Any(enemy =>
        {
            if (!enemy) return false;
            var enemyAttackManager = enemy.GetComponent<AttackManager>();
            return enemyAttackManager.Playing;
        });
        StateMachine.SetBool(EnemiesAttacking, enemiesAttacking);
    }

    public override string[] GetParameters()
    {
        return new[] { EnemiesAttackingParameter };
    }
}