using System;
using UnityEngine;

public class EscapePattern : BasePattern
{
    public Tag targetTag;
    public float speedMultiplier;

    private Action onStop;

    private static readonly int StuckParameter = Animator.StringToHash("stuck");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var escapeBehaviour = animator.GetEntity().GetBehaviour<EscapeBehaviour>();
        var player = EntityManager.Instance.GetEntity(targetTag);
        if (!player)
        {
            return;
        }

        escapeBehaviour.Play(new EscapeBehaviour.Command
            { target = (MovableEntity)player.GetEntity(), speedMultiplier = speedMultiplier }
        );

        onStop = () => animator.SetTrigger(StuckParameter);
        escapeBehaviour.MovableEntity.OnStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var escapeBehaviour = animator.GetEntity().GetBehaviour<EscapeBehaviour>();

        escapeBehaviour.MovableEntity.OnStuck -= onStop;
        escapeBehaviour.Stop();
    }
}