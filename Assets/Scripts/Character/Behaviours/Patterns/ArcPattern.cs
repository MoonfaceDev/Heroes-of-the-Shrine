using System;
using UnityEngine;

public class ArcPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private IModifier speedModifier;
    private EventListener circleEvent;
    private Action onStop;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        WalkBehaviour walkBehaviour = animator.GetComponent<WalkBehaviour>();
        MovableObject player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        Vector3 playerPosition = player.position;
        Vector3 initialDistance = walkBehaviour.MovableObject.position - playerPosition;
        initialDistance.y = 0;
        float radius = initialDistance.magnitude;
        float clockwise = Mathf.Sign(UnityEngine.Random.Range(-1f ,1f));

        circleEvent = EventManager.Attach(() => true, () => {
            Vector3 distance = walkBehaviour.MovableObject.position - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            Vector3 newPosition = playerPosition + distance;
            newPosition.y = walkBehaviour.MovableObject.position.y;
            walkBehaviour.MovableObject.position = newPosition;
            Vector3 direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(direction.x, direction.z, false);
            if ((player.position - walkBehaviour.MovableObject.position).x != 0) {
                walkBehaviour.LookDirection = Mathf.RoundToInt(Mathf.Sign((player.position - walkBehaviour.MovableObject.position).x));
            };
        }, false);

        onStop = () => animator.SetTrigger("stuck");
        walkBehaviour.MovableObject.onStuck += onStop;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        WalkBehaviour walkBehaviour = animator.GetComponent<WalkBehaviour>();

        walkBehaviour.MovableObject.onStuck -= onStop;
        EventManager.Detach(circleEvent);
        walkBehaviour.speed.RemoveModifier(speedModifier);
        walkBehaviour.Stop();
        walkBehaviour.MovableObject.velocity = Vector3.zero;
    }
}
