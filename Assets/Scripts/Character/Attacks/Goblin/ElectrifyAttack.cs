using System.Collections;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Active phase has two parts - periodic and explosion.
/// After the periodic part is checking for hits a certain number of times, the explosion part starts.
/// </summary>
public class ElectrifyAttack : BaseAttack
{
    public SimpleAttack.AttackFlow attackFlow;

    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public ChainHitExecutor periodicHitExecutor;

    [Header("Explosion")] [SerializeInterface] [SerializeReference]
    public BaseHitDetector explosionHitDetector;

    public ChainHitExecutor explosionHitExecutor;
    [SerializeField] public ExtEvent onExplosion;

    private string switchDetectorsListener;

    private void Start()
    {
        PlayEvents.onStop += () =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StopDetector();
            Cancel(switchDetectorsListener);
        };

        explosionHitDetector.onHit += onExplosion.Invoke;
    }

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        StartHitDetector(periodicHitDetector, periodicHitExecutor);

        switchDetectorsListener = InvokeWhen(() => detectCount >= periodicHitCount, () =>
        {
            periodicHitDetector.StopDetector();
            StartHitDetector(explosionHitDetector, explosionHitExecutor);
            detectCount = 0;
        });

        yield return new WaitForSeconds(attackFlow.activeDuration);

        explosionHitDetector.StopDetector();
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}