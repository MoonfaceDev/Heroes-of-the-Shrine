using UnityEngine;

public class ElectrifiedEffect : BaseEffect
{
    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener stopEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Activate(float duration, float speedReductionMultiplier)
    {
        RunBehaviour runBehaviour = GetComponent<RunBehaviour>();
        if (runBehaviour)
        {
            runBehaviour.Stop();
        }
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        if (slideBehaviour)
        {
            slideBehaviour.Stop();
        }
        DodgeBehaviour dodgeBehaviour = GetComponent<DodgeBehaviour>();
        if (dodgeBehaviour)
        {
            dodgeBehaviour.Stop();
        }
        if (active)
        {
            Stop();
        }

        active = true;
        InvokeOnActivate();

        if (walkBehaviour)
        {
            speedModifier = new MultiplierModifier(speedReductionMultiplier);
            walkBehaviour.speed.AddModifier(speedModifier);
        }
        particles.Play();

        startTime = Time.time;
        currentDuration = duration;
        stopEvent = eventManager.Attach(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        active = false;

        if (walkBehaviour)
        {
            walkBehaviour.speed.RemoveModifier(speedModifier);
        }
        particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

        currentDuration = 0;
        eventManager.Detach(stopEvent);
        InvokeOnStop();
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}
