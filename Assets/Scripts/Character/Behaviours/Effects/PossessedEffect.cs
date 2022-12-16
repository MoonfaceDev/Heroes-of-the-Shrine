using UnityEngine;

public class PossessedEffect : BaseEffect
{
    private float currentDuration;
    private float currentStartTime;
    
    public override bool CanPlay()
    {
        return base.CanPlay() && AllStopped(typeof(PossessedEffect));
    }

    public void Play(float maxDuration)
    {
        if (!CanPlay())
        {
            return;
        }
        
        Active = true;
        onPlay.Invoke();

        currentStartTime = Time.time;
        currentDuration = maxDuration;
        EventManager.Attach(() => Time.time - currentStartTime > currentDuration, Stop, false);

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit += OnHit;
        }
        
        DisableBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour), typeof(ForcedBehaviour));
        StopBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour), typeof(ForcedBehaviour));
        MovableObject.velocity = Vector3.zero;
    }

    private void OnHit(float damage)
    {
        Stop();
    }

    public void ReduceDuration(float durationPart)
    {
        currentDuration -= durationPart * currentDuration;
    }

    public override void Stop()
    {
        if (!Active) return;
        Active = false;
        onStop.Invoke();

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit -= OnHit;
        }

        currentDuration = 0;

        EnableBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour), typeof(ForcedBehaviour));
    }

    public override float GetProgress()
    {
        return currentDuration > 0 ? (Time.time - currentStartTime) / currentDuration : 0;
    }
}