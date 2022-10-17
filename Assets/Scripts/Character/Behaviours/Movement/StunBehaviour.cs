using System;
using System.Collections;
using UnityEngine;

public class StunBehaviour : ForcedBehaviour
{
    public int stunFrames;

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

    public override bool Playing => stun;

    private bool _stun;
    private int _stunFrame;
    private Coroutine stopCoroutine;

    public void Play(float time)
    {
        if (!CanPlay())
        {
            return;
        }

        StopBehaviours(typeof(BaseMovementBehaviour), typeof(ForcedBehaviour), typeof(AttackManager));
        movableObject.velocity = Vector3.zero;

        stun = true;
        stunFrame = (stunFrame + 1) % stunFrames;
        InvokeOnPlay();
        movableObject.velocity = Vector3.zero;
        stopCoroutine = StartCoroutine(StopAfter(time));
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        stun = false;
        InvokeOnStop();
    }

    public override void Stop()
    {
        if (stun)
        {
            InvokeOnStop();
            stun = false;
            if (stopCoroutine != null)
            {
                StopCoroutine(stopCoroutine);
            }
        }
    }
}
