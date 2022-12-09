using UnityEngine;

public class SlideBehaviour : BaseMovementBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public bool Slide
    {
        get => slide;
        private set { 
            slide = value;
            Animator.SetBool(SlideParameter, slide);
        }
    }

    public override bool Playing => Slide;

    private bool slide;
    private EventListener stopEvent;
    
    private static readonly int SlideParameter = Animator.StringToHash("slide");

    public override bool CanPlay()
    {
        return base.CanPlay()
            && AllStopped(typeof(AttackManager), typeof(JumpBehaviour), typeof(DodgeBehaviour))
            && MovableObject.velocity.x != 0;
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
        float slideDirection = MovableObject.rotation;
        MovableObject.velocity.x = slideDirection * slideSpeedMultiplier * Mathf.Abs(MovableObject.velocity.x);
        MovableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        MovableObject.velocity.z = 0;
        stopEvent = EventManager.Attach(
            () => Mathf.Approximately(Mathf.Sign(MovableObject.velocity.x), Mathf.Sign(MovableObject.acceleration.x)),
            Stop
        );
    }

    public override void Stop()
    {
        if (!Slide) return;
        InvokeOnStop();
        EventManager.Detach(stopEvent);
        Slide = false;
        MovableObject.velocity.x = 0;
        MovableObject.acceleration.x = 0;
        EnableBehaviours(typeof(WalkBehaviour));
    }
}
