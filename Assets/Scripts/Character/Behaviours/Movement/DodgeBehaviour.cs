using System;
using UnityEngine;

public class DodgeBehaviour : SoloMovementBehaviour
{
    public float dodgeDistance;
    public float anticipateTime;
    public float recoveryTime;

    public event Action onDodge;
    public event Action onRecover;

    public bool recovering
    {
        get => _recovering;
        private set
        {
            _recovering = value;
            animator.SetBool("recoveringFromDodge", _recovering);
        }
    }
    public bool anticipating
    {
        get => _anticipating;
        private set
        {
            _anticipating = value;
            animator.SetBool("anticipatingDodge", _anticipating);
        }
    }

    public override bool Playing => anticipating || recovering;

    private bool _anticipating;
    private bool _recovering;
    private EventListener anticipateEvent;
    private EventListener recoverEvent;

    public override bool CanPlay()
    {
        return base.CanPlay() && movableObject.velocity.z != 0 && movableObject.velocity.x == 0;
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        DisableBehaviours(typeof(WalkBehaviour));
        StopBehaviours(typeof(WalkBehaviour));
        anticipating = true;
        InvokeOnPlay();
        float dodgeDirection = Mathf.Sign(movableObject.velocity.z);
        movableObject.acceleration = Vector3.zero;
        movableObject.velocity = Vector3.zero;
        anticipateEvent = eventManager.StartTimeout(() =>
        {
            anticipating = false;
            movableObject.UpdatePosition(movableObject.position + dodgeDirection * dodgeDistance * Vector3.forward);
            onDodge?.Invoke();
            recovering = true;
            recoverEvent = eventManager.StartTimeout(Stop, recoveryTime);
        }, anticipateTime);
    }

    public override void Stop()
    {
        if (Playing)
        {
            InvokeOnStop();
            EnableBehaviours(typeof(WalkBehaviour));
        }
        if (anticipating)
        {
            eventManager.Detach(anticipateEvent);
            anticipating = false;
        }
        if (recovering)
        {
            eventManager.Detach(recoverEvent);
            recovering = false;
            onRecover?.Invoke();
        }
    }
}
