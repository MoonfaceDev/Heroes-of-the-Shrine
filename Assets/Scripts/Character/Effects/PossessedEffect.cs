using System;
using UnityEngine;

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
    
    private static readonly Type[] BehavioursToBlock = { typeof(BaseAttack), typeof(IMovementBehaviour), typeof(IForcedBehaviour) };

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !IsPlaying<PossessedEffect>();
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentStartTime = Time.time;
        currentDuration = command.maxDuration;
        stopListener = InvokeWhen(() => Time.time - currentStartTime > currentDuration, Stop);

        var hittableBehaviour = GetBehaviour<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit += OnHit;
        }

        BlockBehaviours(BehavioursToBlock);
        StopBehaviours(BehavioursToBlock);
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

        var hittableBehaviour = GetBehaviour<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit -= OnHit;
        }

        currentDuration = 0;

        UnblockBehaviours(BehavioursToBlock);
        Cancel(stopListener);
    }

    public override float GetProgress()
    {
        return currentDuration > 0 ? (Time.time - currentStartTime) / currentDuration : 0;
    }
}