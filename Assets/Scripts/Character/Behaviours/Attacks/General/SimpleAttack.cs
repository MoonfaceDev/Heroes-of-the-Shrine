using System;
using System.Collections;
using UnityEngine;

public class SimpleAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float activeDuration;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;

    public BaseHitDetector hitDetector;

    public SimpleHitExecutor hitExecutor;

    public void Start()
    {
        PlayEvents.onStop.AddListener(hitDetector.StopDetector);
    }

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        hitDetector.StartDetector(hittable => hitExecutor.Execute(this, hittable), AttackManager.hittableTags);
        yield return new WaitForSeconds(attackFlow.activeDuration);
        hitDetector.StopDetector();
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}