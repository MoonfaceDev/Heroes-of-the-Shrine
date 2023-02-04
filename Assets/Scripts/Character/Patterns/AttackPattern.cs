using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

[Serializable]
public class AttackNode
{
    [Inherits(typeof(BaseAttack))] public TypeReference attackType;
    public float startTime;
}

public class AttackPattern : BasePattern
{
    public Tag targetTag;
    public List<AttackNode> attacks;

    private Coroutine attackCoroutine;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var player = EntityManager.Instance.GetEntity(targetTag);

        if (!player)
        {
            return;
        }

        var entity = animator.GetEntity();
        entity.rotation = Mathf.RoundToInt(Mathf.Sign((player.WorldPosition - entity.WorldPosition).x));
        attackCoroutine = EventManager.Instance.StartCoroutine(AttackCoroutine(animator));
    }

    private IEnumerator AttackCoroutine(Animator animator)
    {
        var startTime = Time.time;

        foreach (var node in attacks)
        {
            yield return new WaitForSeconds(node.startTime - (Time.time - startTime));
            var attack = animator.GetEntity().GetBehaviour(node.attackType, exactType: true) as BaseAttack;
            if (attack == null) continue;
            attack.Play(new BaseAttack.Command());
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if (attackCoroutine != null)
        {
            EventManager.Instance.StopCoroutine(attackCoroutine);
        }
    }

    private void OnDestroy()
    {
        if (attackCoroutine != null)
        {
            EventManager.Instance.StopCoroutine(attackCoroutine);
        }
    }
}