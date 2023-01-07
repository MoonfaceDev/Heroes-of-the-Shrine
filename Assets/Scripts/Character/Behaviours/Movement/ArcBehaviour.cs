using UnityEngine;

public class ArcCommand : ICommand
{
    public readonly MovableObject target;
    public readonly float speedMultiplier;

    public ArcCommand(MovableObject target, float speedMultiplier)
    {
        this.target = target;
        this.speedMultiplier = speedMultiplier;
    }
}

[RequireComponent(typeof(WalkBehaviour))]
public class ArcBehaviour : BaseMovementBehaviour<ArcCommand>
{
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string circleListener;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        walkBehaviour.PlayEvents.onStop.AddListener(Stop);
    }

    protected override void DoPlay(ArcCommand command)
    {
        Active = true;

        currentSpeedMultiplier = command.speedMultiplier;
        walkBehaviour.speed *= command.speedMultiplier;

        var playerPosition = command.target.WorldPosition;
        var initialDistance = walkBehaviour.MovableObject.WorldPosition - playerPosition;
        initialDistance.y = 0;
        var radius = initialDistance.magnitude;
        var clockwise = Mathf.Sign(Random.Range(-1f, 1f));

        circleListener = Register(() =>
        {
            var distance = walkBehaviour.MovableObject.WorldPosition - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            var newPosition = playerPosition + distance;
            newPosition.y = walkBehaviour.MovableObject.WorldPosition.y;
            walkBehaviour.MovableObject.WorldPosition = newPosition;
            var direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(new WalkCommand(direction.x, direction.z, false));
            if ((command.target.WorldPosition - walkBehaviour.MovableObject.WorldPosition).x != 0)
            {
                walkBehaviour.MovableObject.rotation = Mathf.RoundToInt(
                    Mathf.Sign((command.target.WorldPosition - walkBehaviour.MovableObject.WorldPosition).x));
            }
        });
    }

    protected override void DoStop()
    {
        Active = false;
        Unregister(circleListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableObject.velocity = Vector3.zero;
    }
}