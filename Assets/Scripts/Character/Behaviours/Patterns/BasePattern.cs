using System;
using UnityEngine;

public abstract class BasePattern : StateMachineBehaviour
{
    public bool hasRandomExitTime = false;
    public float minTime;
    public float maxTime;

    private float timeout;

    public EventManager eventManager
    {
        get => FindObjectOfType<EventManager>();
    }

    public event Action onEnter;
    public event Action onExit;

    private float time;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Debug.Log(animator.name + " starting " + GetType().Name);
        onEnter?.Invoke();
        time = 0;
        if (hasRandomExitTime)
        {
            timeout = UnityEngine.Random.Range(minTime, maxTime);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        time += Time.deltaTime;
        if (hasRandomExitTime && time > timeout)
        {
            animator.SetTrigger("timeout");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Debug.Log(animator.name + " exited " + GetType().Name + " after " + time + "s");
        onExit?.Invoke();
    }
}
