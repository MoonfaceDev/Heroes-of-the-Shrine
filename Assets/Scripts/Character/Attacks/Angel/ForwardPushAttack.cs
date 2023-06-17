using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardPushAttack : BaseAttack
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
    public float speed;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector1;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector2;

    public ChainHitExecutor hitExecutor1;

    public ChainHitExecutor hitExecutor2;

    private List<string> currentTimeouts;

    private void Start()
    {
        phaseEvents.onFinishActive += () =>
        {
            MovableEntity.velocity.x = 0;

            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            foreach (var timeout in currentTimeouts)
            {
                Cancel(timeout);
            }

            currentTimeouts.Clear();
        };
    }

    private void ConfigureHitDetector(BaseHitDetector hitDetector, ChainHitExecutor hitExecutor, float startTime,
        float duration)
    {
        currentTimeouts.Add(StartTimeout(() =>
        {
            StartHitDetector(hitDetector, hitExecutor);
            currentTimeouts.Add(StartTimeout(hitDetector.StopDetector, duration));
        }, startTime));
    }

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.velocity.x = speed * Entity.rotation;
        currentTimeouts = new List<string>();
        ConfigureHitDetector(hitDetector1, hitExecutor1, attackFlow.detector1StartTime, attackFlow.detector1Duration);
        ConfigureHitDetector(hitDetector2, hitExecutor2, attackFlow.detector2StartTime, attackFlow.detector2Duration);
        yield return new WaitForSeconds(attackFlow.activeDuration);
    }
}