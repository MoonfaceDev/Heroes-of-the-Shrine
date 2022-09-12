using System;
using System.Collections;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public event Action onStart;
    public event Action onStop;
    public bool stun
    {
        get => _stun;
        private set
        {
            _stun = value;
            animator.SetBool("stun", _stun);
        }
    }
    
    private bool _stun;
    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private AttackManager attackManager;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        attackManager = GetComponent<AttackManager>();
    }

    public void Stun(float time)
    {
        if (walkBehaviour)
        {
            walkBehaviour.Stop(true);
        }
        if (jumpBehaviour)
        {
            jumpBehaviour.Stop(waitForLand: false);
        }
        if (attackManager)
        {
            attackManager.Stop();
        }
        stun = true;
        onStart?.Invoke();
        movableObject.velocity = new Vector3(0, 0, 0);
        StartCoroutine(StopAfter(time));
    }

    public void Stop()
    {
        stun = false;
        onStop?.Invoke();
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }
}
