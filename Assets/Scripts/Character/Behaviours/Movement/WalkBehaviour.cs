using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : CharacterBehaviour
{
    public float defaultSpeed;

    public delegate void OnStart();
    public delegate void OnStop();

    [HideInInspector] public float speed;
    public event OnStart onStart;
    public event OnStop onStop;
    public bool walk
    {
        get => _walk;
        private set
        {
            _walk = value;
            animator.SetBool("walk", _walk);
        }
    }

    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private AttackManager attackManager;
    private bool _walk; //walking or running

    public override void Awake()
    {
        base.Awake();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
        attackManager = GetComponent<AttackManager>();
        speed = defaultSpeed;
    }

    public bool CanWalk()
    {
        return
            !(jumpBehaviour && (jumpBehaviour.anticipating || jumpBehaviour.recovering))
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager && attackManager.attacking);
    }

    public void Walk(float xAxis, float zAxis)
    {
        if (!CanWalk())
        {
            return;
        }
        // run callbacks
        if (new Vector2(xAxis, zAxis) == Vector2.zero)
        {
            Stop();
        }
        else if (!walk) //first walking frame
        {
            walk = true;
            onStart?.Invoke();
        }
        // move speed
        movableObject.velocity.x = xAxis * speed;
        movableObject.velocity.z = zAxis * speed;
        // look direction
        if (xAxis != 0)
        {
            lookDirection = Mathf.RoundToInt(xAxis);
        }
    }

    public void Stop(bool freeze = false)
    {
        walk = false;
        if (freeze)
        {
            movableObject.velocity.x = 0;
            movableObject.velocity.z = 0;
        }
        onStop?.Invoke();
    }
}
