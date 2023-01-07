using UnityEngine;

public class SlideCommand : ICommand
{
    public readonly int direction;

    public SlideCommand(int direction)
    {
        this.direction = direction;
    }
}

public class SlideBehaviour : BaseMovementBehaviour<SlideCommand>
{
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

    public override bool CanPlay(SlideCommand command)
    {
        var attackManager = GetComponent<AttackManager>();
        return base.CanPlay(command)
               && !IsPlaying<JumpBehaviour>() && !IsPlaying<SlideBehaviour>() && !IsPlaying<DodgeBehaviour>()
               && !(attackManager && !attackManager.IsInterruptible());
    }

    protected override void DoPlay(SlideCommand command)
    {
        if (command.direction == 0)
        {
            return;
        }

        StopBehaviours(typeof(WalkBehaviour), typeof(BaseAttack));
        DisableBehaviours(typeof(WalkBehaviour));

        Slide = true;

        MovableObject.rotation = command.direction;
        MovableObject.velocity.x = command.direction * slideSpeedMultiplier * GetComponent<WalkBehaviour>().speed;
        MovableObject.acceleration.x = -command.direction * slideStopAcceleration;
        MovableObject.velocity.z = 0;
        stopListener = InvokeWhen(
            () => !Mathf.Approximately(Mathf.Sign(MovableObject.velocity.x), command.direction),
            Stop
        );
    }

    protected override void DoStop()
    {
        Cancel(stopListener);
        Slide = false;
        MovableObject.velocity.x = 0;
        MovableObject.acceleration.x = 0;
        EnableBehaviours(typeof(WalkBehaviour));
    }
}