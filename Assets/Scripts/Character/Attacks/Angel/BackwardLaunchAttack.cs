using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardLaunchAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float activeDuration;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;
    public float speed;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    private void Start()
    {
        PlayEvents.onStop += FinishActive;
    }

    private void FinishActive()
    {
        hitDetector.StopDetector();
        MovableEntity.velocity.x = 0;
    }


    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.velocity.x = -speed * Entity.rotation;
        StartHitDetector(hitDetector, hitExecutor);
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}