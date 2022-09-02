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
    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
            animator.SetBool("walk", _active);
        }
    }

    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private AttackManager attackManager;
    private bool _active; //walking or running

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
            && !(slideBehaviour && slideBehaviour.active)
            && !(knockbackBehaviour && (knockbackBehaviour.active || knockbackBehaviour.recovering))
            && !(stunBehaviour && stunBehaviour.active)
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
        else if (!active) //first walking frame
        {
            active = true;
            onStart?.Invoke();
        }
        // move speed
        movableObject.velocity.x = xAxis * speed;
        movableObject.velocity.z = zAxis * speed;
    }

    public void Stop()
    {
        active = false;
        onStop?.Invoke();
    }
}
