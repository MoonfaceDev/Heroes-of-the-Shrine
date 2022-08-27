using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class WalkBehaviour : CharacterBehaviour
{
    public float defaultSpeed;

    public delegate void OnStart();
    public delegate void OnStop();

    [HideInInspector] public float speed;
    public OnStart onStart;
    public OnStop onStop;
    public bool walk
    {
        get { return _walk; }
        set
        {
            _walk = value;
            animator.SetBool("walk", _walk);
            if (value)
            {
                onStart();
            }
            else
            {
                onStop();
            }
        }
    }

    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private bool _walk; //walking or running

    private void Start()
    {
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
        speed = defaultSpeed;
    }

    public bool CanWalk()
    {
        return
            !(jumpBehaviour && (jumpBehaviour.anticipatingJump || jumpBehaviour.recoveringFromJump))
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && (knockbackBehaviour.knockback || knockbackBehaviour.recoveringFromKnockback))
            && !(stunBehaviour && stunBehaviour.stun);
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
            walk = false;
        }
        else if (!walk) //first walking frame
        {
            walk = true;
        }
        // move speed
        movableObject.velocity.x = xAxis * speed;
        movableObject.velocity.z = zAxis * speed;
    }
}
