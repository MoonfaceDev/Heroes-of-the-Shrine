using UnityEngine;

public class WalkCommand : ICommand
{
    public readonly float xAxis;
    public readonly float zAxis;
    public readonly bool fitLookDirection;

    public WalkCommand(float xAxis, float zAxis, bool fitLookDirection = true)
    {
        this.xAxis = xAxis;
        this.zAxis = zAxis;
        this.fitLookDirection = fitLookDirection;
    }
}

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : BaseMovementBehaviour<WalkCommand>
{
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

    public override bool CanPlay(WalkCommand command)
    {
        return base.CanPlay(command) && new Vector2(command.xAxis, command.zAxis) != Vector2.zero;
    }

    protected override void DoPlay(WalkCommand command)
    {
        Walk = true;

        // move speed
        MovableObject.velocity.x = command.xAxis * speed;
        MovableObject.velocity.z = command.zAxis * speed;

        // look direction
        if (command.xAxis != 0 & command.fitLookDirection)
        {
            MovableObject.rotation = Mathf.RoundToInt(Mathf.Sign(command.xAxis));
        }
    }

    protected override void DoStop()
    {
        Walk = false;
        MovableObject.velocity.x = 0;
        MovableObject.velocity.z = 0;
    }
}