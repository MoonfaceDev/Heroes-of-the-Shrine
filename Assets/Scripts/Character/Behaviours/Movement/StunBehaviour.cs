using System;
using System.Collections;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public bool resistant;

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
    private SlideBehaviour slideBehaviour;
    private DodgeBehaviour dodgeBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private AttackManager attackManager;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        dodgeBehaviour = GetComponent<DodgeBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        attackManager = GetComponent<AttackManager>();
    }

    public bool CanReceive()
    {
        return !resistant;
    }

    public void Stun(float time)
    {
        if (!CanReceive())
        {
            return;
        }

        if (walkBehaviour)
        {
            walkBehaviour.Stop(true);
        }

        if (jumpBehaviour)
        {
            jumpBehaviour.Stop(waitForLand: false);
        }

        if (slideBehaviour)
        {
            slideBehaviour.Stop();
        }

        if (dodgeBehaviour)
        {
            dodgeBehaviour.Stop();
        }

        if (knockbackBehaviour)
        {
            knockbackBehaviour.Stop();
        }

        if (attackManager)
        {
            attackManager.Stop();
        }

        Stop();

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
