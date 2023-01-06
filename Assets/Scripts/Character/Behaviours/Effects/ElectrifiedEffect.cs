using System;
using UnityEngine;

public class ElectrifiedEffect : BaseEffect
{
    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string stopListener;

    private static readonly Type[] DisabledBehaviours = { typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(JumpBehaviour) };

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Play(float duration, float speedReductionMultiplier)
    {
        StopBehaviours(typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(ElectrifiedEffect));

        Active = true;
        onPlay.Invoke();

        currentSpeedMultiplier = speedReductionMultiplier;
        if (walkBehaviour)
        {
            walkBehaviour.speed *= currentSpeedMultiplier;
        }
        DisableBehaviours(DisabledBehaviours);
        particles.Play();

        startTime = Time.time;
        currentDuration = duration;
        stopListener = InvokeWhen(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        if (Active)
        {
            onStop.Invoke();
            Active = false;

            if (walkBehaviour)
            {
                walkBehaviour.speed /= currentSpeedMultiplier;
            }
            EnableBehaviours(DisabledBehaviours);
            particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

            currentDuration = 0;
            Cancel(stopListener);
        }
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}
