using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapePattern : BasePattern
{
    public MovableObject player;
    public float speedMultiplier;
    public float minDistance;

    private EscapeBehaviour escapeBehaviour;

    public override void Awake()
    {
        base.Awake();
        escapeBehaviour = GetComponent<EscapeBehaviour>();
    }

    public override void StartPattern()
    {
        base.StartPattern();
        escapeBehaviour.Escape(player, speedMultiplier, minDistance);
    }

    public override void StopPattern()
    {
        base.StopPattern();
        escapeBehaviour.Stop();
    }
}
