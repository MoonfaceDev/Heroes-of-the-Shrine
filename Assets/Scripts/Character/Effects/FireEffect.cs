using System.Collections;
using UnityEngine;

public class FireEffect : BaseEffect<FireEffect.Command>
{
    public class Command
    {
        public readonly float duration;
        public readonly float hitInterval;
        public readonly ChainHitExecutor hitExecutor;
        public readonly BaseAttack relatedAttack;
    }

    private Coroutine damageCoroutine;
    private float startTime;
    private float currentDuration;
    private string stopListener;

    protected override void DoPlay(Command command)
    {
        Active = true;
        damageCoroutine = StartCoroutine(ExecuteHits(command.hitInterval, command.hitExecutor, command.relatedAttack));
        startTime = Time.time;
        currentDuration = command.duration;
        stopListener = eventManager.InvokeWhen(() => Time.time - startTime > command.duration, Stop);
    }

    protected override void DoStop()
    {
        Active = false;

        StopCoroutine(damageCoroutine);

        currentDuration = 0;
        eventManager.Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }

    private IEnumerator ExecuteHits(float hitInterval, ChainHitExecutor hitExecutor, BaseAttack relatedAttack)
    {
        var hittableBehaviour = GetBehaviour<HittableBehaviour>();
        while (true)
        {
            yield return new WaitForSeconds(hitInterval);
            hittableBehaviour.Hit(hitExecutor, new Hit { source = relatedAttack, victim = hittableBehaviour });
        }
    }
}