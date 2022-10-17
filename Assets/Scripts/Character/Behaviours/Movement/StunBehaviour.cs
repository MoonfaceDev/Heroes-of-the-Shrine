using System.Collections;
using UnityEngine;

public class StunBehaviour : ForcedBehaviour
{
    public int stunFrames;

    public bool Stun
    {
        get => stun;
        private set
        {
            stun = value;
            Animator.SetBool("stun", stun);
        }
    }

    public int StunFrame
    {
        get => stunFrame;
        private set
        {
            stunFrame = value;
            Animator.SetInteger("stunFrame", stunFrame);
        }
    }

    public override bool Playing => Stun;

    private bool stun;
    private int stunFrame;
    private Coroutine stopCoroutine;

    public void Play(float time)
    {
        if (!CanPlay())
        {
            return;
        }

        StopBehaviours(typeof(BaseMovementBehaviour), typeof(ForcedBehaviour), typeof(AttackManager));
        MovableObject.velocity = Vector3.zero;

        Stun = true;
        StunFrame = (StunFrame + 1) % stunFrames;
        InvokeOnPlay();
        MovableObject.velocity = Vector3.zero;
        stopCoroutine = StartCoroutine(StopAfter(time));
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Stun = false;
        InvokeOnStop();
    }

    public override void Stop()
    {
        if (Stun)
        {
            InvokeOnStop();
            Stun = false;
            if (stopCoroutine != null)
            {
                StopCoroutine(stopCoroutine);
            }
        }
    }
}
