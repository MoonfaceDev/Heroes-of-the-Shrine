using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Simple implementation of <see cref="BaseAttack"/>, which has a single hit detector, hit executor, and all attack phases have a fixed duration
/// </summary>
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

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    private void Start()
    {
        PlayEvents.onStop += hitDetector.StopDetector;
    }

    protected override IEnumerator ActivePhase()
    {
        StartHitDetector(hitDetector, hitExecutor);

        yield return new WaitForSeconds(attackFlow.activeDuration);

        hitDetector.StopDetector();
    }
}