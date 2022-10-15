using UnityEngine;

public class ElectrifiedEffect : BaseEffect
{
    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;

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
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        if (stunBehaviour)
        {
            stunBehaviour.Stop();
        }
        if (active)
        {
            Deactivate();
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
        eventManager.Attach(() => Time.time - startTime > duration, Deactivate);
    }

    public override void Deactivate()
    {
        active = false;

        if (walkBehaviour)
        {
            walkBehaviour.speed.RemoveModifier(speedModifier);
        }
        particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

        currentDuration = 0;
        InvokeOnDeactivate();
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}
