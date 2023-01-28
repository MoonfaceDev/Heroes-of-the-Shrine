using UnityEngine;

public class SlideBehaviour : BaseMovementBehaviour<SlideBehaviour.Command>
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
               && !IsPlaying<JumpBehaviour>() && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && command.direction != 0
               && AttackManager.CanPlayMove(true);
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(WalkBehaviour), typeof(BaseAttack));
        DisableBehaviours(typeof(WalkBehaviour));

        Slide = true;

        MovableEntity.rotation = command.direction;
        MovableEntity.velocity.x = command.direction * slideSpeedMultiplier * GetComponent<WalkBehaviour>().speed;
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
        EnableBehaviours(typeof(WalkBehaviour));
    }
}