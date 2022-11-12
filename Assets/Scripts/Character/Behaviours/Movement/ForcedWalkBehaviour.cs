using System;
using UnityEngine;

[RequireComponent(typeof(FollowBehaviour))]
public class ForcedWalkBehaviour : PlayableBehaviour
{
    private bool active;
    private Action startWalking;

    public override bool Playing => active;

    public void Play(Vector3 point)
    {
        if (!CanPlay())
        {
            return;
        }
        StopBehaviours(typeof(PlayableBehaviour));
        DisableBehaviours(typeof(PlayerController));
        active = true;
        InvokeOnPlay();

        var walkBehaviour = GetComponent<WalkBehaviour>();
        var followBehaviour = GetComponent<FollowBehaviour>();
        followBehaviour.Play(point);
        startWalking = () =>
        {
            walkBehaviour.OnStop -= startWalking;
            var direction = (point - walkBehaviour.MovableObject.WorldPosition).normalized;
            walkBehaviour.Play(direction.x, direction.z);
        };
        walkBehaviour.OnStop += startWalking;
    }

    public override void Stop()
    {
        if (!active) return;
        InvokeOnStop();
        active = false;
        GetComponent<WalkBehaviour>().OnStop -= startWalking;
        StopBehaviours(typeof(WalkBehaviour));
        EnableBehaviours(typeof(PlayerController));
        MovableObject.velocity = Vector3.zero;
    }
}
