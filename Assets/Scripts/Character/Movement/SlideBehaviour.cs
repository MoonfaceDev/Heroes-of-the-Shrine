using UnityEngine;

public class SlideBehaviour : CharacterBehaviour
{
    public float slideSpeedMultiplier;
    public float slideStopAcceleration;

    public delegate void OnStart();
    public delegate void OnStop();

    public OnStart onStart;
    public OnStop onStop;
    public bool slide
    {
        get { return _slide; }
        set { 
            _slide = value;
            animator.SetBool("slide", _slide);
            if (value)
            {
                onStart();
            }
            else
            {
                onStop();
            }
        }
    }

    private bool _slide;
    private WalkBehaviour walkBehaviour;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanSlide()
    {
        return movableObject.velocity.x != 0 
            && movableObject.position.y == 0
            && !slide
            && !(knockbackBehaviour && (knockbackBehaviour.knockback || knockbackBehaviour.recoveringFromKnockback))
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
            walkBehaviour.walk = false;
        }
        float slideDirection = lookDirection;
        movableObject.velocity.x = slideDirection * slideSpeedMultiplier * movableObject.velocity.x;
        movableObject.acceleration.x = -slideDirection * slideStopAcceleration;
        movableObject.velocity.z = 0;
        slide = true;
        eventManager.Callback(
            () => Mathf.Sign(movableObject.velocity.x) == Mathf.Sign(movableObject.acceleration.x),
            EndSlide
        );
    }

    public void EndSlide()
    {
        slide = false;
        lookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
        movableObject.velocity.x = 0;
        movableObject.acceleration.x = 0;
    }
}
