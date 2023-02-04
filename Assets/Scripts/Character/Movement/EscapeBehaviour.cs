using UnityEngine;

public class EscapeBehaviour : BaseMovementBehaviour<EscapeBehaviour.Command>
{
    public class Command
    {
        public readonly GameEntity target;
        public readonly float speedMultiplier;
        public readonly bool fitRotation;

        public Command(GameEntity target, float speedMultiplier, bool fitRotation = true)
        {
            this.target = target;
            this.speedMultiplier = speedMultiplier;
            this.fitRotation = fitRotation;
        }
    }
    
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string escapeListener;

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
    }

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

        escapeListener = Register(() =>
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
        Unregister(escapeListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        walkBehaviour.Stop();
        MovableEntity.velocity = Vector3.zero;
    }
}