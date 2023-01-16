using UnityEngine;

public class EscapeCommand : ICommand
{
    public readonly MovableEntity target;
    public readonly float speedMultiplier;
    public readonly bool fitLookDirection;

    public EscapeCommand(MovableEntity target, float speedMultiplier, bool fitLookDirection = true)
    {
        this.target = target;
        this.speedMultiplier = speedMultiplier;
        this.fitLookDirection = fitLookDirection;
    }
}

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : BaseMovementBehaviour<EscapeCommand>
{
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string escapeListener;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        MovableEntity.OnStuck += Stop;
        walkBehaviour.PlayEvents.onStop.AddListener(Stop);
    }

    protected override void DoPlay(EscapeCommand command)
    {
        Active = true;

        currentSpeedMultiplier = command.speedMultiplier;
        walkBehaviour.speed *= currentSpeedMultiplier;

        escapeListener = Register(() =>
        {
            var distance = MovableEntity.WorldPosition - command.target.WorldPosition;
            distance.y = 0;
            var direction = distance.normalized;
            walkBehaviour.Play(new WalkCommand(direction.x, direction.z, command.fitLookDirection));
            MovableEntity.rotation = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        });
    }

    protected override void DoStop()
    {
        Active = false;
        Unregister(escapeListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}