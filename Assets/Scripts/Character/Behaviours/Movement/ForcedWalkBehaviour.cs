using UnityEngine;

[RequireComponent(typeof(FollowBehaviour))]
public class ForcedWalkBehaviour : PlayableBehaviour
{
    private bool active;
    private string stopListener;

    public override bool Playing => active;

    public void Play(Vector3 point, float wantedDistance = 0.1f)
    {
        if (!CanPlay())
        {
            return;
        }

        StopBehaviours(typeof(PlayableBehaviour));
        DisableBehaviours(typeof(CharacterController), typeof(RunBehaviour));

        active = true;
        onPlay.Invoke();

        GetComponent<FollowBehaviour>().Play(point);
        stopListener = InvokeWhen(() => MovableObject.GroundDistance(point) < wantedDistance, () =>
        {
            MovableObject.position = point;
            Stop();
        });
    }

    public override void Stop()
    {
        if (!active) return;

        onStop.Invoke();
        active = false;

        Cancel(stopListener);
        StopBehaviours(typeof(FollowBehaviour));
        EnableBehaviours(typeof(PlayerController), typeof(RunBehaviour));

        MovableObject.velocity = Vector3.zero;
    }
}