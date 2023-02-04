using System;
using System.Linq;
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

    private static readonly Type[] BehavioursToBlock =
        { typeof(RunBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour), typeof(JumpBehaviour) };

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(BehavioursToBlock.Append(typeof(ElectrifiedEffect)).ToArray());

        Active = true;

        currentSpeedMultiplier = command.speedReductionMultiplier;
        if (walkBehaviour)
        {
            walkBehaviour.speed *= currentSpeedMultiplier;
        }

        BlockBehaviours(BehavioursToBlock);
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

        UnblockBehaviours(BehavioursToBlock);
        particles.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);

        currentDuration = 0;
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}