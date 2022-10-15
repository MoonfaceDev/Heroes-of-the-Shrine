using System.Collections;
using UnityEngine;

public class FireEffect : BaseEffect
{
    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;
    private EventListener stopEvent;

    public void Activate(float duration, float hitInterval, float damagePerHit)
    {
        active = true;
        InvokeOnActivate();
        damageCoroutine = StartCoroutine(DoDamage(hitInterval, damagePerHit));
        startTime = Time.time;
        currentDuration = duration;
        stopEvent = eventManager.Attach(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        active = false;
        currentDuration = 0;
        StopCoroutine(damageCoroutine);
        eventManager.Detach(stopEvent);
        InvokeOnStop();
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }

    private IEnumerator DoDamage(float hitInterval, float damagePerHit)
    {
        HittableBehaviour hittableBehaviour = GetComponent<HittableBehaviour>();
        while (true)
        {
            yield return new WaitForSeconds(hitInterval);
            hittableBehaviour.Hit(damagePerHit);
        }
    }
}
