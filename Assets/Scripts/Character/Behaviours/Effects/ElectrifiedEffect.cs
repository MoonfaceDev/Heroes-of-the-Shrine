using UnityEngine;

public class ElectrifiedEffect : BaseEffect
{
    private float startTime;
    private float currentDuration;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Activate(float duration, float speedReductionMultiplier)
    {
        active = true;
        InvokeOnActivate();

        speedModifier = new MultiplierModifier(speedReductionMultiplier);
        if (walkBehaviour)
        {
            walkBehaviour.speed.AddModifier(speedModifier);
        }

        startTime = Time.time;
        currentDuration = duration;
        eventManager.Attach(() => Time.time - startTime > duration, Deactivate);
    }

    public override void Deactivate()
    {
        active = false;

        if (walkBehaviour)
        {
            walkBehaviour.speed.RemoveModifier(speedModifier);
        }

        currentDuration = 0;
        InvokeOnDeactivate();
    }

    public override float GetProgress()
    {
        return currentDuration != 0 ? (Time.time - startTime) / currentDuration : 0;
    }
}
