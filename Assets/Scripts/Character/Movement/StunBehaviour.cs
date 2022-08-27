using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public delegate void OnStart();
    public delegate void OnStop();

    public OnStart onStart;
    public OnStop onStop;
    public bool stun
    {
        get
        {
            return _stun;
        }
        set
        {
            _stun = value;
            animator.SetBool("stun", _stun);
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
    
    private bool _stun;
    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
    }

    public void Stun(float time)
    {
        if (walkBehaviour)
        {
            walkBehaviour.walk = false;
        }
        if (jumpBehaviour)
        {
            jumpBehaviour.anticipatingJump = false;
            jumpBehaviour.recoveringFromJump = false;
            jumpBehaviour.EndJump();
        }
        stun = true;
        movableObject.velocity = new Vector3(0, 0, 0);
        StartCoroutine(RemoveStunAfter(time));
    }

    public void RemoveStun()
    {
        stun = false;
    }

    private IEnumerator RemoveStunAfter(float time)
    {
        yield return new WaitForSeconds(time);
        RemoveStun();
    }
}
