using System.Collections;
using ExtEvents;
using UnityEngine;

public class ElectrifyAttack : BaseAttack
{
    public SimpleAttack.AttackFlow attackFlow;

    [Header("Periodic hits")] public PeriodicAbsoluteHitDetector periodicHitDetector;
    public int periodicHitCount;
    public ElectricHitExecutor periodicHitExecutor;

    [Header("Explosion")] [SerializeInterface] [SerializeReference]
    public BaseHitDetector explosionHitDetector;

    public ElectricExplosionHitExecutor explosionHitExecutor;
    [SerializeField] public ExtEvent onExplosion;

    private string switchDetectorsListener;

    public void Start()
    {
        PlayEvents.onStop += () =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StopDetector();
            Cancel(switchDetectorsListener);
        };
    }

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        float detectCount = 0;
        periodicHitDetector.OnDetect += () => detectCount++;

        periodicHitDetector.StartDetector(hittable => periodicHitExecutor.Execute(this, hittable),
            AttackManager.hittableTags);

        switchDetectorsListener = InvokeWhen(() => detectCount >= periodicHitCount, () =>
        {
            periodicHitDetector.StopDetector();
            explosionHitDetector.StartDetector(hittable =>
                {
                    explosionHitExecutor.Execute(this, hittable);
                    onExplosion.Invoke();
                },
                AttackManager.hittableTags);
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