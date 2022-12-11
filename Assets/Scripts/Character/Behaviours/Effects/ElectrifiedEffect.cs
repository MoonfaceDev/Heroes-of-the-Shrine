using System;
using UnityEngine;

public class ElectrifiedEffect : BaseEffect
{
    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener stopEvent;

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

        if (walkBehaviour)
        {
            speedModifier = new MultiplierModifier(speedReductionMultiplier);
            walkBehaviour.speed.AddModifier(speedModifier);
        }
        DisableBehaviours(DisabledBehaviours);
        particles.Play();

        startTime = Time.time;
        currentDuration = duration;
        stopEvent = EventManager.Attach(() => Time.time - startTime > duration, Stop);
    }

    public override void Stop()
    {
        if (Active)
        {
            onStop.Invoke();
            Active = false;

            if (walkBehaviour)
            {
                walkBehaviour.speed.RemoveModifier(speedModifier);
            }
            EnableBehaviours(DisabledBehaviours);
            particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

            currentDuration = 0;
            EventManager.Detach(stopEvent);
        }
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}
