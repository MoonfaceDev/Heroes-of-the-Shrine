using System.Collections;
using UnityEngine;

public class FireEffectCommand : ICommand
{
    public readonly float duration;
    public readonly float hitInterval;
    public readonly float damagePerHit;

    public FireEffectCommand(float duration, float hitInterval, float damagePerHit)
    {
        this.duration = duration;
        this.hitInterval = hitInterval;
        this.damagePerHit = damagePerHit;
    }
}

public class FireEffect : BaseEffect<FireEffectCommand>
{
    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;
    private string stopListener;

    protected override void DoPlay(FireEffectCommand command)
    {
        Active = true;
        damageCoroutine = StartCoroutine(DoDamage(command.hitInterval, command.damagePerHit));
        startTime = Time.time;
        currentDuration = command.duration;
        stopListener = InvokeWhen(() => Time.time - startTime > command.duration, Stop);
    }

    protected override void DoStop()
    {
        Active = false;

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