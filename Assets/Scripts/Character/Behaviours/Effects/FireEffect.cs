using System.Collections;
using UnityEngine;

public class FireEffect : BaseEffect
{
    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;
    private string stopListener;

    public void Play(float duration, float hitInterval, float damagePerHit)
    {
        Active = true;
        onPlay.Invoke();
        damageCoroutine = StartCoroutine(DoDamage(hitInterval, damagePerHit));
        startTime = Time.time;
        currentDuration = duration;
        stopListener = InvokeWhen(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        if (!Active) return;
        Active = false;
        onStop.Invoke();

        StopCoroutine(damageCoroutine);

        currentDuration = 0;
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }

    private IEnumerator DoDamage(float hitInterval, float damagePerHit)
    {
        var hittableBehaviour = GetComponent<HittableBehaviour>();
        while (true)
        {
            yield return new WaitForSeconds(hitInterval);
            hittableBehaviour.Hit(damagePerHit);
        }
    }
}
