using UnityEngine;

public class FollowPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        MovableObject player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();
        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();

        followBehaviour.Play(player, speedMultiplier);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();

        followBehaviour.Stop();
    }
}
