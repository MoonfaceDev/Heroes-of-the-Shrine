using System;
using UnityEngine;

public class ElectrifiedEffectCommand : ICommand
{
    public readonly float duration;
    public readonly float speedReductionMultiplier;

    public ElectrifiedEffectCommand(float duration, float speedReductionMultiplier)
    {
        this.duration = duration;
        this.speedReductionMultiplier = speedReductionMultiplier;
    }
}

public class ElectrifiedEffect : BaseEffect<ElectrifiedEffectCommand>
{
    public ParticleSystem particles;

    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string stopListener;

    private static readonly Type[] DisabledBehaviours =
        { typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(JumpBehaviour) };

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    protected override void DoPlay(ElectrifiedEffectCommand command)
    {
        StopBehaviours(typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(ElectrifiedEffect));

        Active = true;

        currentSpeedMultiplier = command.speedReductionMultiplier;
        if (walkBehaviour)
        {
            walkBehaviour.speed *= currentSpeedMultiplier;
        }

        DisableBehaviours(DisabledBehaviours);
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

        EnableBehaviours(DisabledBehaviours);
        particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

        currentDuration = 0;
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}