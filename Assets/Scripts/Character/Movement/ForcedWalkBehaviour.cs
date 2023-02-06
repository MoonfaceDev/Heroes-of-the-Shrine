using UnityEngine;

public class ForcedWalkBehaviour : PlayableBehaviour<ForcedWalkBehaviour.Command>
{
    public class Command
    {
        public readonly Vector3 point;
        public readonly float wantedDistance;

        public Command(Vector3 point, float wantedDistance = 0.1f)
        {
            this.point = point;
            this.wantedDistance = wantedDistance;
        }
    }

    
    private bool active;
    private string stopListener;

    public override bool Playing => active;

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(IPlayableBehaviour));
        DisableBehaviours(typeof(CharacterController));
        BlockBehaviours(typeof(RunBehaviour));

        active = true;

        GetBehaviour<AutoWalkBehaviour>().Play(new AutoWalkBehaviour.Command(command.point));
        stopListener = InvokeWhen(() => MovableEntity.GroundDistance(command.point) < command.wantedDistance, () =>
        {
            MovableEntity.UpdateWorldPosition(command.point);
            Stop();
        });
    }

    protected override void DoStop()
    {
        active = false;

        Cancel(stopListener);
        StopBehaviours(typeof(AutoWalkBehaviour));
        EnableBehaviours(typeof(CharacterController));
        UnblockBehaviours(typeof(RunBehaviour));

        MovableEntity.velocity = Vector3.zero;
    }
}