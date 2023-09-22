using System.Collections;
using UnityEngine;

/// <summary>
/// Simple implementation of <see cref="BaseAttack"/>, which has a single hit detector, hit executor, and all attack phases have a fixed duration
/// </summary>
public class SimpleAttack : BaseAttack
{
    public float activeDuration;
    
    public HitSource hitSource;

    private void Start()
    {
        phaseEvents.onFinishActive += hitSource.Stop;
    }

    protected override IEnumerator ActivePhase()
    {
        hitSource.Start(this);
        yield return new WaitForSeconds(activeDuration);
    }
}