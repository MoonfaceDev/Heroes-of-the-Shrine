using UnityEngine;

public class ForcedWalkCommand : ICommand
{
    public readonly Vector3 point;
    public readonly float wantedDistance;

    public ForcedWalkCommand(Vector3 point, float wantedDistance = 0.1f)
    {
        this.point = point;
        this.wantedDistance = wantedDistance;
    }
}

[RequireComponent(typeof(AutoWalkBehaviour))]
public class ForcedWalkBehaviour : PlayableBehaviour<ForcedWalkCommand>
{
    private bool active;
    private string stopListener;

    public override bool Playing => active;

    protected override void DoPlay(ForcedWalkCommand command)
    {
        StopBehaviours(typeof(IPlayableBehaviour));
        DisableBehaviours(typeof(CharacterController), typeof(RunBehaviour));

        active = true;

        GetComponent<AutoWalkBehaviour>().Play(new AutoWalkCommand(command.point));
        stopListener = InvokeWhen(() => MovableEntity.GroundDistance(command.point) < command.wantedDistance, () =>
        {
            MovableEntity.position = command.point;
            Stop();
        });
    }

    protected override void DoStop()
    {
        active = false;

        Cancel(stopListener);
        StopBehaviours(typeof(AutoWalkBehaviour));
        EnableBehaviours(typeof(PlayerController), typeof(RunBehaviour));

        MovableEntity.velocity = Vector3.zero;
    }
}