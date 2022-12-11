using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : BaseMovementBehaviour
{
    public float defaultSpeed;

    public ModifiableValue speed;
    
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
        speed = new ModifiableValue(defaultSpeed);
    }

    public void Play(float xAxis, float zAxis, bool fitLookDirection = true)
    {
        if (!CanPlay())
        {
            return;
        }
        // move speed
        MovableObject.velocity.x = xAxis * speed;
        MovableObject.velocity.z = zAxis * speed;
        // look direction
        if (xAxis != 0 & fitLookDirection)
        {
            MovableObject.rotation = Mathf.RoundToInt(Mathf.Sign(xAxis));
        }
        // run callbacks
        if (new Vector2(xAxis, zAxis) == Vector2.zero)
        {
            Stop();
        }
        else if (!Walk) //first walking frame
        {
            Walk = true;
            onPlay.Invoke();
        }
    }

    public override void Stop()
    {
        if (!Walk) return;
        Walk = false;
        onStop.Invoke();
    }
}
