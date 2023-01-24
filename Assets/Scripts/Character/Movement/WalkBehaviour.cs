using UnityEngine;

[RequireComponent(typeof(MovableEntity))]
public class WalkBehaviour : BaseMovementBehaviour<WalkBehaviour.Command>
{
    public class Command
    {
        public readonly Vector2 direction;
        public readonly bool fitLookDirection;

        public Command(Vector2 direction, bool fitLookDirection = true)
        {
            this.direction = direction.normalized;
            this.fitLookDirection = fitLookDirection;
        }

        public Command(Vector3 direction, bool fitLookDirection = true)
            : this(MathUtils.ToPlane(direction), fitLookDirection)
        {
        }
    }

    
    public float defaultSpeed;

    public float speed;

    public bool Walk
    {
        get => walk;
        private set
        {
            walk = value;
            Animator.SetBool(WalkParameter, walk);
        }
    }

    public override bool Playing => Walk;

    private bool walk; //walking or running

    private static readonly int WalkParameter = Animator.StringToHash("walk");

    public override void Awake()
    {
        base.Awake();
        speed = defaultSpeed;
    }

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && command.direction != Vector2.zero;
    }

    protected override void DoPlay(Command command)
    {
        Walk = true;

        // move speed
        MovableEntity.velocity.x = command.direction.x * speed;
        MovableEntity.velocity.z = command.direction.y * speed;

        // look direction
        if (command.direction.x != 0 & command.fitLookDirection)
        {
            MovableEntity.rotation = Mathf.RoundToInt(Mathf.Sign(command.direction.x));
        }
    }

    protected override void DoStop()
    {
        Walk = false;
        MovableEntity.velocity.x = 0;
        MovableEntity.velocity.z = 0;
    }
}