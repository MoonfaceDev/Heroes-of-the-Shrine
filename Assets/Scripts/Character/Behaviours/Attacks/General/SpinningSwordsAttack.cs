using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public SimpleHitExecutor hitExecutor;

    private List<string> currentTimeouts;

    public void Start()
    {
        PlayEvents.onStop.AddListener(() =>
        {
            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            foreach (var timeout in currentTimeouts)
            {
                Cancel(timeout);
            }

            currentTimeouts.Clear();
        });
    }

    private void ConfigureHitDetector(BaseHitDetector hitDetector, float startTime, float duration)
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
        ConfigureHitDetector(hitDetector1, attackFlow.detector1StartTime, attackFlow.detector1Duration);
        ConfigureHitDetector(hitDetector2, attackFlow.detector2StartTime, attackFlow.detector2Duration);
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}