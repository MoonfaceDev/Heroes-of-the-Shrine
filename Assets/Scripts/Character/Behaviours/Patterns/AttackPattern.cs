using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackNode
{
    public BaseAttack attack;
    public float startTime;
}

[RequireComponent(typeof(FollowBehaviour))]
[RequireComponent(typeof(EscapeBehaviour))]
public class AttackPattern : BasePattern
{
    public MovableObject player;
    public float followSpeedMultiplier;
    public float attackDistance;
    public List<AttackNode> attacks;
    public float overallAttackTime;
    public bool escapeAfterAttack;
    public float escapeSpeedMultiplier;
    public float escapeTime;

    private FollowBehaviour followBehaviour;
    private EscapeBehaviour escapeBehaviour;

    public override void Awake()
    {
        base.Awake();
        followBehaviour = GetComponent<FollowBehaviour>();
        escapeBehaviour = GetComponent<EscapeBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();
        followBehaviour.Follow(player, followSpeedMultiplier, attackDistance);
        followBehaviour.onStop += () => StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        float startTime = Time.time;

        foreach (AttackNode node in attacks)
        {
            yield return new WaitForSeconds(node.startTime - (Time.time - startTime));
            try
            {
                node.attack.Attack();
            }
            catch (CannotAttackException)
            {
                Debug.LogError("[" + (Time.time - startTime) + "s] Cannot execute " + node.attack.attackName);
            }
        }

        yield return new WaitForSeconds(overallAttackTime - (Time.time - startTime));

        if (escapeAfterAttack)
        {
            escapeBehaviour.Escape(player, escapeSpeedMultiplier, 0);
            yield return new WaitForSeconds(escapeTime);
            escapeBehaviour.Stop();
        }
        StopPattern();
    }

    public override void StopPattern()
    {
        base.StopPattern();
        followBehaviour.Stop();
        escapeBehaviour.Stop();
    }
}
