using System;
using UnityEngine;

public abstract class BasePattern : StateMachineBehaviour
{
    public EventManager eventManager
    {
        get => FindObjectOfType<EventManager>();
    }

    private float time;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Debug.Log(animator.name + " starting " + GetType().Name);
        time = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        time += Time.deltaTime;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Debug.Log(animator.name + " exited " + GetType().Name + " after " + time + "s");
        time = 0;
    }

    public void Exit(Animator animator)
    {
        animator.SetTrigger("exit");
    }
}
