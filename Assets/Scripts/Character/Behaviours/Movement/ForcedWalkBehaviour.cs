using System;
using UnityEngine;

[RequireComponent(typeof(FollowBehaviour))]
public class ForcedWalkBehaviour : PlayableBehaviour
{
    private bool active;
    
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

        GetComponent<FollowBehaviour>().Play(point);
    }

    public override void Stop()
    {
        if (!active) return;
        
        InvokeOnStop();
        active = false;
        
        StopBehaviours(typeof(FollowBehaviour));
        EnableBehaviours(typeof(PlayerController));
        
        MovableObject.velocity = Vector3.zero;
    }
}