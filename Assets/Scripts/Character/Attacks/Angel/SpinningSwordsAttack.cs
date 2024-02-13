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
        public float activeDuration;
        public float detector1StartTime;
        public float detector1Duration;
        public float detector2StartTime;
        public float detector2Duration;
    }

    public AttackFlow attackFlow;
    
    public HitSource hitSource1;
    public HitSource hitSource2;

    private List<string> currentTimeouts;

    private void Start()
    {
        phaseEvents.onFinishActive += () =>
        {
            hitSource1.Stop();
            hitSource2.Stop();
            foreach (var timeout in currentTimeouts)
            {
                eventManager.Cancel(timeout);
            }

            currentTimeouts.Clear();
        };
    }

    private void ConfigureHitDetector(HitSource hitSource, float startTime, float duration)
    {
        currentTimeouts.Add(eventManager.StartTimeout(() =>
        {
            hitSource.Start(this);
            currentTimeouts.Add(eventManager.StartTimeout(hitSource.Stop, duration));
        }, startTime));
    }

    protected override IEnumerator ActivePhase()
    {
        currentTimeouts = new List<string>();
        ConfigureHitDetector(hitSource1, attackFlow.detector1StartTime, attackFlow.detector1Duration);
        ConfigureHitDetector(hitSource2, attackFlow.detector2StartTime, attackFlow.detector2Duration);
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }
}