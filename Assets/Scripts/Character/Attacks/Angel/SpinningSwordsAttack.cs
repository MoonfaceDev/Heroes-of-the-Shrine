using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack that has two hitboxes, each one has different timing inside the active phase
/// </summary>
public class SpinningSwordsAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float activeDuration;
        public float detector1StartTime;
        public float detector1Duration;
        public float detector2StartTime;
        public float detector2Duration;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector1;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector2;

    public ChainHitExecutor hitExecutor1;

    public ChainHitExecutor hitExecutor2;

    private List<string> currentTimeouts;

    private void Start()
    {
        attackEvents.onFinishActive += () =>
        {
            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            foreach (var timeout in currentTimeouts)
            {
                Cancel(timeout);
            }

            currentTimeouts.Clear();
        };
    }

    private void ConfigureHitDetector(BaseHitDetector hitDetector, IHitExecutor hitExecutor, float startTime,
        float duration)
    {
        currentTimeouts.Add(StartTimeout(() =>
        {
            hitDetector.StartDetector(hittable => hitExecutor.Execute(this, hittable), AttackManager.hittableTags);
            currentTimeouts.Add(StartTimeout(hitDetector.StopDetector, duration));
        }, startTime));
    }

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        currentTimeouts = new List<string>();
        ConfigureHitDetector(hitDetector1, hitExecutor1, attackFlow.detector1StartTime, attackFlow.detector1Duration);
        ConfigureHitDetector(hitDetector2, hitExecutor2, attackFlow.detector2StartTime, attackFlow.detector2Duration);
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}