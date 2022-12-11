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
        var attackManager = GetComponent<AttackManager>();
        return base.CanPlay() 
               && AllStopped(typeof(JumpBehaviour), typeof(SlideBehaviour), typeof(DodgeBehaviour))
               && !(attackManager && !attackManager.IsInterruptible());
    }

    public void Play(int direction)
    {
        if (!(CanPlay() && direction != 0))
        {
            return;
        }
        
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour), typeof(AttackManager));
        
        Slide = true;
        onPlay.Invoke();

        MovableObject.rotation = direction;
        MovableObject.velocity.x = direction * slideSpeedMultiplier * GetComponent<WalkBehaviour>().speed;
        MovableObject.acceleration.x = -direction * slideStopAcceleration;
        MovableObject.velocity.z = 0;
        stopEvent = EventManager.Attach(
            () => Mathf.Approximately(Mathf.Sign(MovableObject.velocity.x), Mathf.Sign(MovableObject.acceleration.x)),
            Stop
        );
    }

    public override void Stop()
    {
        if (!Slide) return;
        onStop.Invoke();
        EventManager.Detach(stopEvent);
        Slide = false;
        MovableObject.velocity.x = 0;
        MovableObject.acceleration.x = 0;
        EnableBehaviours(typeof(WalkBehaviour));
    }
}
