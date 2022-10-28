using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

[Serializable]
public class AttackNode
{
    [Inherits(typeof(BaseAttack))]
    public TypeReference attackType;
    public float startTime;
}

public class AttackPattern : BasePattern
{
    public string targetTag;
    public List<AttackNode> attacks;

    private Coroutine attackCoroutine;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        if (!player)
        {
            return;
        }

        Character character = animator.GetComponent<Character>();
        character.LookDirection = Mathf.RoundToInt(Mathf.Sign((player.GetComponent<MovableObject>().position - character.movableObject.position).x));
        attackCoroutine = EventManager.StartCoroutine(AttackCoroutine(animator));
    }

    private IEnumerator AttackCoroutine(Animator animator)
    {
        float startTime = Time.time;

        foreach (AttackNode node in attacks)
        {
            yield return new WaitForSeconds(node.startTime - (Time.time - startTime));
            BaseAttack attack = animator.GetComponent(node.attackType) as BaseAttack;
            try
            {
                attack.Play();
            }
            catch (CannotAttackException)
            {
                Debug.LogWarning("[" + (Time.time - startTime) + "s] Cannot execute " + attack.AttackName);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if (attackCoroutine != null)
        {
            EventManager.StopCoroutine(attackCoroutine);
        }
    }

    private void OnDestroy()
    {
        if (EventManager != null && attackCoroutine != null)
        {
            EventManager.StopCoroutine(attackCoroutine);
        }
    }
}
