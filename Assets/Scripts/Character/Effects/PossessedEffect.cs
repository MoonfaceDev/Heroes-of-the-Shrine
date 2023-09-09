using System;
using UnityEngine;

public class PossessedEffect : BaseEffect<PossessedEffect.Command>
{
    public class Command
    {
        public float maxDuration;
    }
    
    private float currentDuration;
    private float currentStartTime;
    private string stopListener;
    
    private static readonly Type[] BehavioursToBlock = { typeof(IControlledBehaviour), typeof(IForcedBehaviour) };

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !IsPlaying<PossessedEffect>();
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentStartTime = Time.time;
        currentDuration = command.maxDuration;
        stopListener = eventManager.InvokeWhen(() => Time.time - currentStartTime > currentDuration, Stop);

        var hittableBehaviour = GetBehaviour<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.onHit += OnHit;
        }

        StopBehaviours(BehavioursToBlock);
        BlockBehaviours(BehavioursToBlock);
        MovableEntity.velocity = Vector3.zero;
    }

    private void OnHit()
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

        var hittableBehaviour = GetBehaviour<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.onHit -= OnHit;
        }

        currentDuration = 0;

        UnblockBehaviours(BehavioursToBlock);
        eventManager.Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration > 0 ? (Time.time - currentStartTime) / currentDuration : 0;
    }
}