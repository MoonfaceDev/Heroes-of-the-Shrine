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

        MovableObject player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();
        Character character = animator.GetComponent<Character>();
        character.LookDirection = Mathf.RoundToInt(Mathf.Sign((player.position - character.movableObject.position).x));
        attackCoroutine = EventManager.StartCoroutine(AttackCoroutine(animator, player));
    }

    private IEnumerator AttackCoroutine(Animator animator, MovableObject player)
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
                Debug.LogError("[" + (Time.time - startTime) + "s] Cannot execute " + attack.AttackName);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.StopCoroutine(attackCoroutine);
    }
}
