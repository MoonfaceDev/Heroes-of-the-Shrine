using System;
using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : CharacterBehaviour
{
    public float defaultSpeed;

    [HideInInspector] public float speed;
    public event Action onStart;
    public event Action onStop;
    public bool walk
    {
        get => _walk;
        private set
        {
            _walk = value;
            animator.SetBool("walk", _walk);
        }
    }

    private bool _walk; //walking or running

    public override void Awake()
    {
        base.Awake();
        speed = defaultSpeed;
    }

    public bool CanWalk()
    {
        JumpBehaviour jumpBehaviour = GetComponent<JumpBehaviour>();
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        DodgeBehaviour dodgeBehaviour = GetComponent<DodgeBehaviour>();
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();
        return
            !(jumpBehaviour && (jumpBehaviour.anticipating || jumpBehaviour.recovering))
            && !(slideBehaviour && slideBehaviour.slide)
            && !(dodgeBehaviour && dodgeBehaviour.dodge)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager && !attackManager.CanWalk());
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
