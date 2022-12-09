using UnityEngine;

public class PossessedEffect : BaseEffect
{
    private float currentDuration;
    private float currentStartTime;

    public void Play(float maxDuration)
    {
        StopBehaviours(typeof(PossessedEffect));
        
        Active = true;
        InvokeOnPlay();

        currentStartTime = Time.time;
        currentDuration = maxDuration;
        EventManager.Attach(() => Time.time - currentStartTime > currentDuration, Stop, false);

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit += OnHit;
        }

        if (IsPlaying(typeof(WalkBehaviour)))
        {
            MovableObject.velocity = Vector3.zero;
        }
        DisableBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour));
        StopBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour));
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
        InvokeOnStop();

        var hittableBehaviour = GetComponent<HittableBehaviour>();
        if (hittableBehaviour)
        {
            hittableBehaviour.OnHit -= OnHit;
        }

        currentDuration = 0;

        EnableBehaviours(typeof(BaseAttack), typeof(BaseMovementBehaviour));
    }

    public override float GetProgress()
    {
        return currentDuration > 0 ? (Time.time - currentStartTime) / currentDuration : 0;
    }
}