using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class EscapeBehaviour : BaseMovementBehaviour
{
    public bool Active { get; private set; }

    public override bool Playing => Active;

    private WalkBehaviour walkBehaviour;
    private float currentSpeedMultiplier;
    private string escapeListener;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        MovableObject.OnStuck += Stop;
        walkBehaviour.onStop.AddListener(Stop);
    }

    public void Play(MovableObject target, float speedMultiplier, bool fitLookDirection = true)
    {
        if (!CanPlay())
        {
            return;
        }

        Active = true;
        onPlay.Invoke();

        currentSpeedMultiplier = speedMultiplier;
        walkBehaviour.speed *= currentSpeedMultiplier;

        escapeListener = Register(() =>
        {
            var distance = MovableObject.WorldPosition - target.WorldPosition;
            distance.y = 0;
            var direction = distance.normalized;
            walkBehaviour.Play(direction.x, direction.z, fitLookDirection);
            MovableObject.rotation = -Mathf.RoundToInt(Mathf.Sign(direction.x));
        });
    }

    public override void Stop()
    {
        if (!Active) return;
        onStop.Invoke();
        Active = false;
        Unregister(escapeListener);
        walkBehaviour.speed /= currentSpeedMultiplier;
        StopBehaviours(typeof(WalkBehaviour));
        MovableObject.velocity = Vector3.zero;
    }
}