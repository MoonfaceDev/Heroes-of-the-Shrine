using UnityEngine;

public class EnemyCountBrainModule : BrainModule
{
    private const string EnemyCountParameter = "enemyCount";
    private static readonly int EnemyCount = Animator.StringToHash(EnemyCountParameter);

    protected override void Update()
    {
        base.Update();
        StateMachine.SetInteger(EnemyCount, EntityManager.Instance.CountEntities(Tag.Enemy));
    }

    public override string[] GetParameters()
    {
        return new[] { EnemyCountParameter };
    }
}