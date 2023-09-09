using UnityEngine;

public class ForcedWalkBehaviour : PlayableBehaviour<ForcedWalkBehaviour.Command>
{
    public class Command
    {
        public Vector3 point;
        public float wantedDistance = 0.1f;
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

        GetBehaviour<AutoWalkBehaviour>().Play(new AutoWalkBehaviour.Command { destination = command.point });
        stopListener = eventManager.InvokeWhen(() => MovableEntity.GroundDistance(command.point) < command.wantedDistance, () =>
        {
            MovableEntity.UpdateWorldPosition(command.point);
            Stop();
        });
    }

    protected override void DoStop()
    {
        active = false;

        eventManager.Cancel(stopListener);
        StopBehaviours(typeof(AutoWalkBehaviour));
        EnableBehaviours(typeof(CharacterController));
        UnblockBehaviours(typeof(RunBehaviour));

        MovableEntity.velocity = Vector3.zero;
    }
}