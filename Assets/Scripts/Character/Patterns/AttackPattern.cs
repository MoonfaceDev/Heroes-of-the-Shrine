using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

public class AttackPattern : BasePattern
{
    [Serializable]
    public class AttackEntry
    {
        [Inherits(typeof(BaseAttack))] public TypeReference attackType;
        public float delayBefore;
    }

    public Tag targetTag;
    public List<AttackEntry> attacks;

    private BaseAttack currentAttack;
    private Action attackStopListener;

    private static readonly int AttackOver = Animator.StringToHash("attackOver");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        NextAttack(animator, 0);
    }

    private void NextAttack(Animator animator, int attackIndex)
    {
        if (attackIndex >= attacks.Count)
        {
            animator.SetTrigger(AttackOver);
            return;
        }

        var entity = animator.GetEntity();
        var attackEntry = attacks[attackIndex];
        currentAttack = (BaseAttack)entity.GetBehaviour(attackEntry.attackType, exactType: true);

        eventManager.StartTimeout(() =>
        {
            RotateToPlayer(entity);
            attackStopListener =
                currentAttack.PlayEvents.onStop.SubscribeOnce(() => NextAttack(animator, attackIndex + 1));
            currentAttack.Play(new BaseAttack.Command());
        }, attackEntry.delayBefore);
    }

    private void RotateToPlayer(GameEntity entity)
    {
        var player = EntityManager.Instance.GetEntity(targetTag);
        entity.WorldRotation = player.WorldPosition - entity.WorldPosition;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        currentAttack.PlayEvents.onStop -= attackStopListener;
    }
}