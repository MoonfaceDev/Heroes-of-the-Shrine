using System;
using System.Threading.Tasks;

public class TimerEffectCondition : IEffectCondition
{
    public TimeSpan duration;

    public event IEffectCondition.OnSet onSet;

    private DateTime startTime;
    private bool manuallySet;

    public TimerEffectCondition(TimeSpan duration)
    {
        this.duration = duration;
        startTime = DateTime.Now;
        Task.Delay(duration).ContinueWith(task =>
        {
            if (!manuallySet)
            {
                onSet?.Invoke();
            }
        });
    }

    public float GetProgress()
    {
        return manuallySet ? 1 : (float)((DateTime.Now - startTime) / duration);
    }

    public bool IsSet()
    {
        return manuallySet || DateTime.Now - startTime < duration;
    }

    public void Set()
    {
        manuallySet = true;
        onSet?.Invoke();
    }
}
