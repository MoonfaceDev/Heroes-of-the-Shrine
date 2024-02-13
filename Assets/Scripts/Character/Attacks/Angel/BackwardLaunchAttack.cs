using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardLaunchAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float activeDuration;
        public float detectorStartTime;
        public float detectorDuration;
    }

    public AttackFlow attackFlow;
    public float speed;

    public HitSource hitSource;

    private List<string> currentTimeouts;

    private void Start()
    {
        phaseEvents.onFinishActive += () =>
        {
            hitSource.Stop();
            MovableEntity.velocity.x = 0;

            foreach (var timeout in currentTimeouts)
            {
                eventManager.Cancel(timeout);
            }

            currentTimeouts.Clear();
        };
    }

    private void ConfigureHitDetector()
    {
        currentTimeouts.Add(eventManager.StartTimeout(() =>
        {
            hitSource.Start(this);
            currentTimeouts.Add(eventManager.StartTimeout(hitSource.Stop, attackFlow.detectorDuration));
        }, attackFlow.detectorStartTime));
    }

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.velocity.x = -speed * Entity.rotation;
        currentTimeouts = new List<string>();
        ConfigureHitDetector();
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }
}