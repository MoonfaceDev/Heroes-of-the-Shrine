using System.Collections;
using UnityEngine;

public class FireEffect : BaseEffect
{
    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;

    public void Activate(float duration, float hitInterval, float damagePerHit)
    {
        active = true;
        InvokeOnActivate();
        damageCoroutine = StartCoroutine(DoDamage(hitInterval, damagePerHit));
        startTime = Time.time;
        currentDuration = duration;
        eventManager.Attach(() => Time.time - startTime > duration, Deactivate);
    }

    public override void Deactivate()
    {
        active = false;
        currentDuration = 0;
        InvokeOnDeactivate();
        StopCoroutine(damageCoroutine);
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
            try
            {
                hittableBehaviour.Hit(damagePerHit);
            }
            catch (CannotHitException)
            {
                print(hittableBehaviour.name + " could not be hit by fire effect");
            }
        }
    }
}
