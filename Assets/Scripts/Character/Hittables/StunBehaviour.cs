using System;
using System.Linq;
using UnityEngine;

public class StunBehaviour : PlayableBehaviour<StunBehaviour.Command>, IForcedBehaviour
{
    public class Command
    {
        public float time;
    }
    
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
    
    private static readonly Type[] BlockedBehaviours = { typeof(IControlledBehaviour) };

    protected override void DoPlay(Command command)
    {
        StopBehaviours(BlockedBehaviours.Append(typeof(IForcedBehaviour)).ToArray());
        BlockBehaviours(BlockedBehaviours);
        
        MovableEntity.velocity = Vector3.zero;

        Stun = true;
        StunFrame = (StunFrame + 1) % stunFrames;
        stopTimeout = StartTimeout(Stop, command.time);
    }

    protected override void DoStop()
    {
        Stun = false;
        Cancel(stopTimeout);
        UnblockBehaviours(BlockedBehaviours);
    }
}