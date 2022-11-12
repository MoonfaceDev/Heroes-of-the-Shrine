using System;
using UnityEngine;

public class EscapePattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private Action onStop;
    
    private static readonly int StuckParameter = Animator.StringToHash("stuck");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var escapeBehaviour = animator.GetComponent<EscapeBehaviour>();
        var player = GameObject.FindGameObjectWithTag(targetTag);
        if (!player)
        {
            return;
        }

        escapeBehaviour.Play(player.GetComponent<MovableObject>(), speedMultiplier);

        onStop = () => animator.SetTrigger(StuckParameter);
        escapeBehaviour.MovableObject.OnStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var escapeBehaviour = animator.GetComponent<EscapeBehaviour>();

        escapeBehaviour.MovableObject.OnStuck -= onStop;
        escapeBehaviour.Stop();
    }
}
