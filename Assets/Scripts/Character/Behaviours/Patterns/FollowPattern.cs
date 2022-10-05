using UnityEngine;

[RequireComponent(typeof(FollowBehaviour))]
public class FollowPattern : BasePattern
{
    public MovableObject player;
    public float speedMultiplier;
    public float minimumDistance;

    private FollowBehaviour followBehaviour;

    public override void Awake()
    {
        base.Awake();
        followBehaviour = GetComponent<FollowBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();
        followBehaviour.Follow(player, speedMultiplier, minimumDistance);
    }

    public override void StopPattern()
    {
        base.StopPattern();
        followBehaviour.Stop();
    }
}
