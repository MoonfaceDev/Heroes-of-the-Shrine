using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : BaseMovementBehaviour<EscapeBehaviour.Command>
{
    public class Command
    {
        public readonly MovableEntity target;
        public readonly float speedMultiplier;
        public readonly bool fitRotation;

        public Command(MovableEntity target, float speedMultiplier, bool fitRotation = true)
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
        walkBehaviour = GetComponent<WalkBehaviour>();
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