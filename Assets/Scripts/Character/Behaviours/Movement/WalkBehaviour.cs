using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : BaseMovementBehaviour
{
    public float defaultSpeed;

    [HideInInspector] public ModifiableValue speed;
    public bool walk
    {
        get => _walk;
        private set
        {
            _walk = value;
            animator.SetBool("walk", _walk);
        }
    }

    public override bool Playing => walk;

    private bool _walk; //walking or running

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
        // run callbacks
        if (new Vector2(xAxis, zAxis) == Vector2.zero)
        {
            InvokeOnStop();
            walk = false;
        }
        else if (!walk) //first walking frame
        {
            walk = true;
            InvokeOnPlay();
        }
        // move speed
        movableObject.velocity.x = xAxis * speed;
        movableObject.velocity.z = zAxis * speed;
        // look direction
        if (xAxis != 0 & fitLookDirection)
        {
            lookDirection = Mathf.RoundToInt(Mathf.Sign(xAxis));
        }
    }

    public override void Stop()
    {
        if (walk)
        {
            InvokeOnStop();
            walk = false;
        }
    }
}
