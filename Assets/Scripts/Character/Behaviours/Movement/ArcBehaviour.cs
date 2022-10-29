using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]

public class ArcBehaviour : BaseMovementBehaviour
{
    public bool Active
    {
        get => active;
        private set => active = value;
    }

    public override bool Playing => Active;

    private bool active;
    private WalkBehaviour walkBehaviour;
    private IModifier speedModifier;
    private EventListener circleEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    public void Start()
    {
        walkBehaviour.OnStop += Stop;
    }

    public void Play(MovableObject target, float speedMultiplier, bool fitLookDirection = true)
    {
        if (!CanPlay())
        {
            return;
        }
        Active = true;
        InvokeOnPlay();

        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        Vector3 playerPosition = target.position;
        Vector3 initialDistance = walkBehaviour.MovableObject.position - playerPosition;
        initialDistance.y = 0;
        float radius = initialDistance.magnitude;
        float clockwise = Mathf.Sign(Random.Range(-1f, 1f));

        circleEvent = EventManager.Attach(() => true, () => {
            Vector3 distance = walkBehaviour.MovableObject.position - playerPosition;
            distance.y = 0;
            distance *= radius / distance.magnitude;
            Vector3 newPosition = playerPosition + distance;
            newPosition.y = walkBehaviour.MovableObject.position.y;
            walkBehaviour.MovableObject.position = newPosition;
            Vector3 direction = clockwise * Vector3.Cross(distance, Vector3.up).normalized;
            walkBehaviour.Play(direction.x, direction.z, false);
            if ((target.position - walkBehaviour.MovableObject.position).x != 0)
            {
                walkBehaviour.LookDirection = Mathf.RoundToInt(Mathf.Sign((target.position - walkBehaviour.MovableObject.position).x));
            };
        }, false);
    }

    public override void Stop()
    {
        if (Active)
        {
            InvokeOnStop();
            Active = false;
            EventManager.Detach(circleEvent);
            walkBehaviour.speed.RemoveModifier(speedModifier);
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        if (EventManager != null && circleEvent != null)
        {
            EventManager.Detach(circleEvent);
        }
    }
}
