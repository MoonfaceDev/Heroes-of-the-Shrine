using UnityEngine;

public class ArcBehaviour : PlayableBehaviour<ArcBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
        public GameEntity target;
        public float speedMultiplier;
    }

    public override bool Playing => active;

    private bool active;
    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string circleListener;

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
    }

    private void Start()
    {
        walkBehaviour.PlayEvents.onStop += Stop;
    }

    protected override void DoPlay(Command command)
    {
        active = true;

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
            walkBehaviour.MovableEntity.UpdateWorldPosition(newPosition);
            var direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(new WalkBehaviour.Command(direction, false));
            if ((command.target.WorldPosition - walkBehaviour.MovableEntity.WorldPosition).x != 0)
            {
                walkBehaviour.MovableEntity.WorldRotation = Mathf.RoundToInt(
                    Mathf.Sign((command.target.WorldPosition - walkBehaviour.MovableEntity.WorldPosition).x));
            }
        });
    }

    protected override void DoStop()
    {
        active = false;
        Unregister(circleListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}