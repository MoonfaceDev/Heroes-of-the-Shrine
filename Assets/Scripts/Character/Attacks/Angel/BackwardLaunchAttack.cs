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
        public float detectorStartTime;
        public float detectorDuration;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;
    public float speed;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    private List<string> currentTimeouts;

    private void Start()
    {
        phaseEvents.onFinishActive += () =>
        {
            hitDetector.StopDetector();
            MovableEntity.velocity.x = 0;

            foreach (var timeout in currentTimeouts)
            {
                Cancel(timeout);
            }

            currentTimeouts.Clear();
        };
    }

    private void ConfigureHitDetector()
    {
        currentTimeouts.Add(StartTimeout(() =>
        {
            StartHitDetector(hitDetector, hitExecutor);
            currentTimeouts.Add(StartTimeout(hitDetector.StopDetector, attackFlow.detectorDuration));
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