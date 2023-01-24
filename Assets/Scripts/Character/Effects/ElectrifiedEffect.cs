using System;
using UnityEngine;

public class ElectrifiedEffect : BaseEffect<ElectrifiedEffect.Command>
{
    public class Command
    {
        public readonly float duration;
        public readonly float speedReductionMultiplier;

        public Command(float duration, float speedReductionMultiplier)
        {
            this.duration = duration;
            this.speedReductionMultiplier = speedReductionMultiplier;
        }
    }

    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string stopListener;

    private static readonly Type[] BehavioursToDisable =
        { typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(JumpBehaviour) };

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(ElectrifiedEffect));

        Active = true;

        currentSpeedMultiplier = command.speedReductionMultiplier;
        if (walkBehaviour)
        {
            walkBehaviour.speed *= currentSpeedMultiplier;
        }

        DisableBehaviours(BehavioursToDisable);
        particles.Play();

        startTime = Time.time;
        currentDuration = command.duration;
        stopListener = InvokeWhen(() => Time.time - startTime > command.duration, Stop);
    }

    protected override void DoStop()
    {
        Active = false;

        if (walkBehaviour)
        {
            walkBehaviour.speed /= currentSpeedMultiplier;
        }

        EnableBehaviours(BehavioursToDisable);
        particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

        currentDuration = 0;
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}