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
            Animator.SetBool(StunParameter, stun);
        }
    }

    public int StunFrame
    {
        get => stunFrame;
        private set
        {
            stunFrame = value;
            Animator.SetInteger(StunFrameParameter, stunFrame);
        }
    }

    public override bool Playing => Stun;

    private bool stun;
    private int stunFrame;
    private Coroutine stopCoroutine;
    private static readonly int StunParameter = Animator.StringToHash("stun");
    private static readonly int StunFrameParameter = Animator.StringToHash("stunFrame");

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
