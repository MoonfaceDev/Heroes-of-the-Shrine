﻿using UnityEngine;

public class PossessedEffect : BaseEffect<PossessedEffect.Command>
{
    public class Command
    {
        public readonly float maxDuration;

        public Command(float maxDuration)
        {
            this.maxDuration = maxDuration;
        }
    }
    
    private float currentDuration;
    private float currentStartTime;
    private string stopListener;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !IsPlaying<PossessedEffect>() && GetComponent<HittableBehaviour>().CanGetHit();
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentStartTime = Time.time;
        currentDuration = command.maxDuration;
        stopListener = InvokeWhen(() => Time.time - currentStartTime > currentDuration, Stop);

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit += OnHit;
        }

        DisableBehaviours(typeof(BaseAttack), typeof(IMovementBehaviour), typeof(IForcedBehaviour));
        StopBehaviours(typeof(BaseAttack), typeof(IMovementBehaviour), typeof(IForcedBehaviour));
        MovableEntity.velocity = Vector3.zero;
    }

    private void OnHit(float damage)
    {
        Stop();
    }

    public void ReduceDuration(float durationPart)
    {
        currentDuration -= durationPart * currentDuration;
    }

    protected override void DoStop()
    {
        Active = false;

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit -= OnHit;
        }

        currentDuration = 0;

        EnableBehaviours(typeof(BaseAttack), typeof(IMovementBehaviour), typeof(IForcedBehaviour));
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration > 0 ? (Time.time - currentStartTime) / currentDuration : 0;
    }
}