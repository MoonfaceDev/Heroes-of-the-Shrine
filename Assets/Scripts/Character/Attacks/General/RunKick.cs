using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
public class RunKick : MotionAttack
{
    public override bool CanPlay(BaseAttackCommand command)
    {
        return base.CanPlay(command) && IsPlaying<RunBehaviour>();
    }
}