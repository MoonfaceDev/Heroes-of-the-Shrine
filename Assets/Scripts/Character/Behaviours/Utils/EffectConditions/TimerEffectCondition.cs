using System;
using UnityEngine;

public class TimerEffectCondition : BaseEffectCondition
{
    public float duration;

    private float startTime;
    private bool manuallySet;

    public TimerEffectCondition(EventManager eventManager, float duration)
    {
        this.duration = duration;
        startTime = Time.time;
        eventManager.StartTimeout(() =>
        {
            if (!manuallySet)
            {
                Set();
            }
        }, duration);
    }

    public override float GetProgress()
    {
        return manuallySet ? 1 : (float)((Time.time - startTime) / duration);
    }

    public override bool IsSet()
    {
        return manuallySet || Time.time - startTime < duration;
    }

    public override void Set()
    {
        manuallySet = true;
        InvokeOnSet();
    }
}
