using UnityEngine;

public class SlideBehaviour : SoloMovementBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public bool Slide
    {
        get => slide;
        private set { 
            slide = value;
            Animator.SetBool("slide", slide);
        }
    }

    public override bool Playing => Slide;

    private bool slide;
    private EventListener stopEvent;

    public override bool CanPlay()
    {
        return base.CanPlay() && MovableObject.velocity.x != 0;
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour));
        Slide = true;
        InvokeOnPlay();
        float slideDirection = LookDirection;
        MovableObject.velocity.x = slideDirection * slideSpeedMultiplier * Mathf.Abs(MovableObject.velocity.x);
        MovableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        MovableObject.velocity.z = 0;
        stopEvent = EventManager.Attach(
            () => Mathf.Sign(MovableObject.velocity.x) == Mathf.Sign(MovableObject.acceleration.x),
            Stop
        );
    }

    public override void Stop()
    {
        if (Slide)
        {
            InvokeOnStop();
            EventManager.Detach(stopEvent);
            Slide = false;
            MovableObject.velocity.x = 0;
            MovableObject.acceleration.x = 0;
            EnableBehaviours(typeof(WalkBehaviour));
        }
    }
}
