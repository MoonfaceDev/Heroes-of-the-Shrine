using System;
using System.Collections;
using UnityEngine;

public class StunBehaviour : CharacterBehaviour
{
    public bool resistant;
    public int stunFrames;

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

    public int stunFrame
    {
        get => _stunFrame;
        private set
        {
            _stunFrame = value;
            animator.SetInteger("stunFrame", _stunFrame);
        }
    }

    private bool _stun;
    private int _stunFrame;
    private Coroutine stopCoroutine;
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
        stunFrame = (stunFrame + 1) % stunFrames;
        onStart?.Invoke();
        movableObject.velocity = Vector3.zero;
        stopCoroutine = StartCoroutine(StopAfter(time));
    }

    public void Stop()
    {
        stun = false;
        onStop?.Invoke();
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
        }
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }
}
