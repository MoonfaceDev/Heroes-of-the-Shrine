using System;
using UnityEngine;

public class ArcPattern : BasePattern
{
    public Tag targetTag;
    public float speedMultiplier;

    private Action onStop;
    private static readonly int StuckParameter = Animator.StringToHash("stuck");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var arcBehaviour = animator.GetEntity().GetBehaviour<ArcBehaviour>();
        var player = EntityManager.Instance.GetEntity(targetTag);
        if (!player)
        {
            return;
        }

        arcBehaviour.Play(new ArcBehaviour.Command
            { target = (MovableEntity)player.GetEntity(), speedMultiplier = speedMultiplier }
        );

        onStop = () => animator.SetTrigger(StuckParameter);
        arcBehaviour.MovableEntity.OnStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        var arcBehaviour = animator.GetEntity().GetBehaviour<ArcBehaviour>();

        arcBehaviour.MovableEntity.OnStuck -= onStop;
        arcBehaviour.Stop();
    }
}