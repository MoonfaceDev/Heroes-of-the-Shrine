using System;
using UnityEngine;

public abstract class BasePattern : StateMachineBehaviour
{
    public bool hasRandomExitTime;
    public float minTime;
    public float maxTime;

    private float timeout;

    public event Action OnEnter;

    private float time;

    private static readonly int TimeoutParameter = Animator.StringToHash("timeout");

    protected readonly EventManager eventManager = new();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        OnEnter?.Invoke();
        time = 0;
        if (hasRandomExitTime)
        {
            timeout = UnityEngine.Random.Range(minTime, maxTime);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        eventManager.Tick();
        time += Time.deltaTime;
        if (hasRandomExitTime && time > timeout)
        {
            animator.SetTrigger(TimeoutParameter);
        }
    }
}