using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class ArcBehaviour : BaseMovementBehaviour<ArcBehaviour.Command>
{
    public class Command
    {
        public readonly MovableEntity target;
        public readonly float speedMultiplier;

        public Command(MovableEntity target, float speedMultiplier)
        {
            this.target = target;
            this.speedMultiplier = speedMultiplier;
        }
    }
    
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string circleListener;

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    private void Start()
    {
        walkBehaviour.PlayEvents.onStop += Stop;
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentSpeedMultiplier = command.speedMultiplier;
        walkBehaviour.speed *= command.speedMultiplier;

        var playerPosition = command.target.WorldPosition;
        var initialDistance = walkBehaviour.MovableEntity.WorldPosition - playerPosition;
        initialDistance.y = 0;
        var radius = initialDistance.magnitude;
        var clockwise = Mathf.Sign(Random.Range(-1f, 1f));

        circleListener = Register(() =>
        {
            var distance = walkBehaviour.MovableEntity.WorldPosition - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            var newPosition = playerPosition + distance;
            newPosition.y = walkBehaviour.MovableEntity.WorldPosition.y;
            walkBehaviour.MovableEntity.WorldPosition = newPosition;
            var direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(new WalkBehaviour.Command(direction, false));
            if ((command.target.WorldPosition - walkBehaviour.MovableEntity.WorldPosition).x != 0)
            {
                walkBehaviour.MovableEntity.rotation = Mathf.RoundToInt(
                    Mathf.Sign((command.target.WorldPosition - walkBehaviour.MovableEntity.WorldPosition).x));
            }
        });
    }

    protected override void DoStop()
    {
        Active = false;
        Unregister(circleListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}