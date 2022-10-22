using System;
using UnityEngine;

public class ArcPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private Action onStop;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        ArcBehaviour arcBehaviour = animator.GetComponent<ArcBehaviour>();
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        if (!player)
        {
            return;
        }

        arcBehaviour.Play(player.GetComponent<MovableObject>(), speedMultiplier);

        onStop = () => animator.SetTrigger("stuck");
        arcBehaviour.MovableObject.OnStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        ArcBehaviour arcBehaviour = animator.GetComponent<ArcBehaviour>();

        arcBehaviour.MovableObject.OnStuck -= onStop;
        arcBehaviour.Stop();
    }
}
