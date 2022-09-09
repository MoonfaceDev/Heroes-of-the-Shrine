using System.Collections;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public delegate void OnStart();
    public delegate void OnStop();

    public event OnStart onStart;
    public event OnStop onStop;
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

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
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
