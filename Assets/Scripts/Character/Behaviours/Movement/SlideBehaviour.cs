using UnityEngine;

public class SlideBehaviour : CharacterBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public delegate void OnStart();
    public delegate void OnStop();

    public event OnStart onStart;
    public event OnStop onStop;
    public bool slide
    {
        get => _slide;
        private set { 
            _slide = value;
            animator.SetBool("slide", _slide);
        }
    }

    private bool _slide;
    private WalkBehaviour walkBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanSlide()
    {
        return movableObject.velocity.x != 0 
            && movableObject.position.y == 0
            && !slide
            && !(knockbackBehaviour && (knockbackBehaviour.knockback || knockbackBehaviour.recovering))
            && !(stunBehaviour && stunBehaviour.stun);
    }

    public void Slide()
    {
        if (!CanSlide())
        {
            return;
        }
        if (walkBehaviour)
        {
            walkBehaviour.Stop();
        }
        slide = true;
        onStart?.Invoke();
        float slideDirection = lookDirection;
        movableObject.velocity.x = slideDirection * slideSpeedMultiplier * Mathf.Abs(movableObject.velocity.x);
        movableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        movableObject.velocity.z = 0;
        eventManager.Callback(
            () => Mathf.Sign(movableObject.velocity.x) == Mathf.Sign(movableObject.acceleration.x),
            Stop
        );
    }

    public void Stop()
    {
        slide = false;
        lookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
        onStop?.Invoke();
    }
}
