using UnityEngine;

public class SlideBehaviour : SoloMovementBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public bool slide
    {
        get => _slide;
        private set { 
            _slide = value;
            animator.SetBool("slide", _slide);
        }
    }

    public override bool Playing => slide;

    private bool _slide;
    private EventListener stopEvent;

    public override bool CanPlay()
    {
        return base.CanPlay() && movableObject.velocity.x != 0;
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour));
        slide = true;
        InvokeOnPlay();
        float slideDirection = lookDirection;
        movableObject.velocity.x = slideDirection * slideSpeedMultiplier * Mathf.Abs(movableObject.velocity.x);
        movableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        movableObject.velocity.z = 0;
        stopEvent = eventManager.Attach(
            () => Mathf.Sign(movableObject.velocity.x) == Mathf.Sign(movableObject.acceleration.x),
            Stop
        );
    }

    public override void Stop()
    {
        if (slide)
        {
            InvokeOnStop();
            eventManager.Detach(stopEvent);
            slide = false;
            movableObject.velocity.x = 0;
            movableObject.acceleration.x = 0;
            EnableBehaviours(typeof(WalkBehaviour));
        }
    }
}
