using System.Collections;
using UnityEngine;

/// <summary>
/// Active phase has two parts - periodic and explosion.
/// After the periodic part is checking for hits a certain number of times, the explosion part starts.
/// </summary>
public class ElectrifyAttack : BaseAttack
{
    public float activeDuration;

    public HitSource electrifyHitSource;
    public float electrifyInterval;
    public int electrifyCount;

    public HitSource explosionHitSource;

    private string electrifyIntervalId;
    private string switchDetectorsListener;

    private void Start()
    {
        PlayEvents.onStop += () =>
        {
            eventManager.Cancel(electrifyIntervalId);
            explosionHitSource.Stop();
            eventManager.Cancel(switchDetectorsListener);
        };
    }

    protected override IEnumerator ActivePhase()
    {
        float detectCount = 0;
        
        void Electrify()
        {
            electrifyHitSource.Start(this);
            electrifyHitSource.Stop();
            detectCount++;
        }
        
        electrifyIntervalId = eventManager.StartInterval(Electrify, electrifyInterval);
        Electrify();

        switchDetectorsListener = eventManager.InvokeWhen(() => detectCount >= electrifyCount, () =>
        {
            eventManager.Cancel(electrifyIntervalId);
            explosionHitSource.Start(this);
        });

        yield return new WaitForSeconds(activeDuration);

        explosionHitSource.Stop();
    }
}