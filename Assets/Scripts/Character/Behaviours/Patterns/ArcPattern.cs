using System;
using UnityEngine;

public class ArcPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private Action onStop;
    private static readonly int StuckParameter = Animator.StringToHash("stuck");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var arcBehaviour = animator.GetComponent<ArcBehaviour>();
        var player = GameObject.FindGameObjectWithTag(targetTag);
        if (!player)
        {
            return;
        }

        arcBehaviour.Play(player.GetComponent<MovableObject>(), speedMultiplier);

        onStop = () => animator.SetTrigger(StuckParameter);
        arcBehaviour.MovableObject.OnStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var arcBehaviour = animator.GetComponent<ArcBehaviour>();

        arcBehaviour.MovableObject.OnStuck -= onStop;
        arcBehaviour.Stop();
    }
}
