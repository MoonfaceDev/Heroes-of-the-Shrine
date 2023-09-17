using UnityEngine;

public class EscapeBehaviour : PlayableBehaviour<EscapeBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
        public GameEntity target;
        public float speedMultiplier;
        public bool fitRotation = true;
    }
    
    public bool Active { get; private set; }

    public override bool Playing => Active;

    [InjectBehaviour] private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string escapeListener;

    private void Start()
    {
        MovableEntity.OnStuck += Stop;
        walkBehaviour.PlayEvents.onStop += Stop;
    }

    protected override void DoPlay(Command command)
    {
        Active = true;

        currentSpeedMultiplier = command.speedMultiplier;
        walkBehaviour.speed *= currentSpeedMultiplier;

        escapeListener = eventManager.Register(() =>
        {
            var distance = MovableEntity.WorldPosition - command.target.WorldPosition;
            distance.y = 0;
            var direction = distance.normalized;
            walkBehaviour.Play(new WalkBehaviour.Command(direction, command.fitRotation));
            MovableEntity.WorldRotation = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        });
    }

    protected override void DoStop()
    {
        Active = false;
        eventManager.Unregister(escapeListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}