using UnityEngine;

public class StunCommand : ICommand
{
    public readonly float time;

    public StunCommand(float time)
    {
        this.time = time;
    }
}

public class StunBehaviour : ForcedBehaviour<StunCommand>
{
    public int stunFrames;

    public bool Stun
    {
        get => stun;
        private set
        {
            stun = value;
            Animator.SetBool(StunParameter, stun);
        }
    }

    public int StunFrame
    {
        get => stunFrame;
        private set
        {
            stunFrame = value;
            Animator.SetInteger(StunFrameParameter, stunFrame);
        }
    }

    public override bool Playing => Stun;

    private bool stun;
    private int stunFrame;
    private string stopTimeout;
    private static readonly int StunParameter = Animator.StringToHash("stun");
    private static readonly int StunFrameParameter = Animator.StringToHash("stunFrame");

    protected override void DoPlay(StunCommand command)
    {
        StopBehaviours(typeof(IMovementBehaviour), typeof(IForcedBehaviour), typeof(BaseAttack));
        MovableObject.velocity = Vector3.zero;

        Stun = true;
        StunFrame = (StunFrame + 1) % stunFrames;
        stopTimeout = StartTimeout(Stop, command.time);
    }

    protected override void DoStop()
    {
        Stun = false;
        Cancel(stopTimeout);
    }
}