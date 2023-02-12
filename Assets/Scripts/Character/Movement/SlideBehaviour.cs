using UnityEngine;

public class SlideBehaviour : PlayableBehaviour<SlideBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
        public readonly int direction;

        public Command(int direction)
        {
            this.direction = direction;
        }
    }
    
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public Cooldown cooldown;

    public bool Slide
    {
        get => slide;
        private set
        {
            slide = value;
            Animator.SetBool(SlideParameter, slide);
        }
    }

    public override bool Playing => Slide;

    private bool slide;
    private string stopListener;

    private static readonly int SlideParameter = Animator.StringToHash("slide");

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command)
               && cooldown.CanPlay()
               && !IsPlaying<JumpBehaviour>()
               && command.direction != 0;
    }

    protected override void DoPlay(Command command)
    {
        cooldown.Reset();
        
        StopBehaviours(typeof(IControlledBehaviour));
        BlockBehaviours(typeof(IControlledBehaviour));

        Slide = true;

        MovableEntity.rotation = command.direction;
        MovableEntity.velocity.x = command.direction * slideSpeedMultiplier * GetBehaviour<WalkBehaviour>().speed;
        MovableEntity.acceleration.x = -command.direction * slideStopAcceleration;
        MovableEntity.velocity.z = 0;
        stopListener = InvokeWhen(
            () => !Mathf.Approximately(Mathf.Sign(MovableEntity.velocity.x), command.direction),
            Stop
        );
    }

    protected override void DoStop()
    {
        Cancel(stopListener);
        Slide = false;
        MovableEntity.velocity.x = 0;
        MovableEntity.acceleration.x = 0;
        UnblockBehaviours(typeof(IControlledBehaviour));
    }
}