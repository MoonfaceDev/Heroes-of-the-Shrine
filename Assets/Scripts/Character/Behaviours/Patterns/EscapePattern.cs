using System;
using UnityEngine;

public class EscapePattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private Action onStop;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        EscapeBehaviour escapeBehaviour = animator.GetComponent<EscapeBehaviour>();
        MovableObject player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();

        escapeBehaviour.Play(player, speedMultiplier);

        onStop = () => animator.SetTrigger("stuck");
        escapeBehaviour.MovableObject.onStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EscapeBehaviour escapeBehaviour = animator.GetComponent<EscapeBehaviour>();

        escapeBehaviour.MovableObject.onStuck -= onStop;
        escapeBehaviour.Stop();
    }
}
