using UnityEngine;

[RequireComponent(typeof(FollowBehaviour))]
public class ForcedWalkBehaviour : PlayableBehaviour
{
    private bool active;
    private EventListener stopEvent;

    public override bool Playing => active;

    public void Play(Vector3 point, float wantedDistance = 0.1f)
    {
        if (!CanPlay())
        {
            return;
        }

        StopBehaviours(typeof(PlayableBehaviour));
        DisableBehaviours(typeof(CharacterController));

        active = true;
        onPlay.Invoke();

        GetComponent<FollowBehaviour>().Play(point);
        stopEvent = EventManager.Instance.Attach(() => MovableObject.GroundDistance(point) < wantedDistance, () =>
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

        EventManager.Instance.Detach(stopEvent);
        StopBehaviours(typeof(FollowBehaviour));
        EnableBehaviours(typeof(PlayerController));

        MovableObject.velocity = Vector3.zero;
    }
}