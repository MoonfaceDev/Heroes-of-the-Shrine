using System.Collections;
using UnityEngine;

/// <summary>
/// Active phase has two parts - periodic and explosion.
/// After the periodic part is checking for hits a certain number of times, the explosion part starts.
/// </summary>
public class ElectrifyAttack : BaseAttack
{
    public float activeDuration;

    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public ChainHitExecutor periodicHitExecutor;

    [Header("Explosion")] [SerializeInterface] [SerializeReference]
    public BaseHitDetector explosionHitDetector;

    public ChainHitExecutor explosionHitExecutor;

    private string switchDetectorsListener;

    private void Start()
    {
        PlayEvents.onStop += () =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StopDetector();
            eventManager.Cancel(switchDetectorsListener);
        };
    }

    protected override IEnumerator ActivePhase()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        switchDetectorsListener = eventManager.InvokeWhen(() => detectCount >= periodicHitCount, () =>
        {
            periodicHitDetector.StopDetector();
            StartHitDetector(explosionHitDetector, explosionHitExecutor);
            detectCount = 0;
        });

        StartHitDetector(periodicHitDetector, periodicHitExecutor);

        yield return new WaitForSeconds(activeDuration);

        explosionHitDetector.StopDetector();
    }
}