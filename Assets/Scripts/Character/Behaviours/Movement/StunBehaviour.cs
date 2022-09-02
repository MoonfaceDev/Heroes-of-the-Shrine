using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public delegate void OnStart();
    public delegate void OnStop();

    public event OnStart onStart;
    public event OnStop onStop;
    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
            animator.SetBool("stun", _active);
        }
    }
    
    private bool _active;
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
            walkBehaviour.Stop();
        }
        if (jumpBehaviour)
        {
            jumpBehaviour.Stop(waitForLand: false);
        }
        active = true;
        onStart?.Invoke();
        movableObject.velocity = new Vector3(0, 0, 0);
        StartCoroutine(StopAfter(time));
    }

    public void Stop()
    {
        active = false;
        onStop?.Invoke();
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }
}
