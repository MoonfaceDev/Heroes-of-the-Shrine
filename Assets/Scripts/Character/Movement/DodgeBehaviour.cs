using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DodgeBehaviour : PhasedBehaviour<DodgeBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
        public int direction;
    }

    [FormerlySerializedAs("dodgeDistance")] public float distance;
    public float anticipateTime;
    public float recoveryTime;

    public Cooldown cooldown;

    private int currentDirection;

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

        currentDirection = command.direction;
        base.DoPlay(command);
    }

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(anticipateTime);
    }

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.UpdatePosition(MovableEntity.position + currentDirection * distance * Vector3.forward);
        yield break;
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(recoveryTime);
    }

    protected override void DoStop()
    {
        base.DoStop();
        UnblockBehaviours(typeof(IControlledBehaviour));
    }
}