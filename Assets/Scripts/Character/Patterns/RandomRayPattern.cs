using UnityEngine;

public class RandomRayPattern : BasePattern
{
    public float speedMultiplier;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed *= speedMultiplier;

        var direction = Random.insideUnitCircle.normalized;
        walkBehaviour.Play(new WalkBehaviour.Command(direction));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed /= speedMultiplier;
        walkBehaviour.Stop();
    }
}