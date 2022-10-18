using UnityEngine;

public class FollowPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        if (!player)
        {
            return;
        }

        followBehaviour.Play(player.GetComponent<MovableObject>(), speedMultiplier);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();

        followBehaviour.Stop();
    }
}
