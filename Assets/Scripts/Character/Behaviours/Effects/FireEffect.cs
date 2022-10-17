using System.Collections;
using UnityEngine;

public class FireEffect : BaseEffect
{
    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;
    private EventListener stopEvent;

    public void Play(float duration, float hitInterval, float damagePerHit)
    {
        active = true;
        InvokeOnPlay();
        damageCoroutine = StartCoroutine(DoDamage(hitInterval, damagePerHit));
        startTime = Time.time;
        currentDuration = duration;
        stopEvent = eventManager.Attach(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        if (active)
        {
            active = false;
            InvokeOnStop();

            StopCoroutine(damageCoroutine);

            currentDuration = 0;
            eventManager.Detach(stopEvent);
        }
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
