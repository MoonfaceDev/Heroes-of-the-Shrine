using UnityEngine;

public class RandomRayPattern : BasePattern
{
    public float speedMultiplier;

    private IModifier speedModifier;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        var direction = Random.insideUnitCircle.normalized;
        walkBehaviour.Play(direction.x, direction.y);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed.RemoveModifier(speedModifier);
    }
}