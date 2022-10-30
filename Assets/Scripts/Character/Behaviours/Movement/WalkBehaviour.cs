using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : BaseMovementBehaviour
{
    public float defaultSpeed;

    [HideInInspector] public ModifiableValue speed;
    public bool Walk
    {
        get => walk;
        private set
        {
            walk = value;
            Animator.SetBool("walk", walk);
        }
    }

    public override bool Playing => Walk;

    private bool walk; //walking or running

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
            LookDirection = Mathf.RoundToInt(Mathf.Sign(xAxis));
        }
        // run callbacks
        if (new Vector2(xAxis, zAxis) == Vector2.zero)
        {
            Stop();
        }
        else if (!Walk) //first walking frame
        {
            Walk = true;
            InvokeOnPlay();
        }
    }

    public override void Stop()
    {
        if (Walk)
        {
            Walk = false;
            InvokeOnStop();
        }
    }
}
