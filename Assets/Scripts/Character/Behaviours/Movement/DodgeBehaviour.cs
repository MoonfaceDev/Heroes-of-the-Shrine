using System;
using UnityEngine;

public class DodgeBehaviour : CharacterBehaviour
{
    public float dodgeDistance;
    public float anticipateTime;
    public float recoveryTime;
    public float cooldown;

    private float lastSlideFinishTime;
    public event Action onAnticipate;
    public event Action onDodge;
    public event Action onRecover;
    public event Action onStop;

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
    public bool dodge
    {
        get => anticipating || recovering;
    }

    private bool _anticipating;
    private bool _recovering;
    private WalkBehaviour walkBehaviour;
    private EventListener anticipateEvent;
    private EventListener recoverEvent;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        lastSlideFinishTime = Time.time - cooldown;
        onStop += () => lastSlideFinishTime = Time.time;
    }

    public bool CanDodge()
    {
        KnockbackBehaviour knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        SlideBehaviour slideBehaviour = GetComponent<SlideBehaviour>();
        StunBehaviour stunBehaviour = GetComponent<StunBehaviour>();
        AttackManager attackManager = GetComponent<AttackManager>();
        return movableObject.velocity.z != 0
            && movableObject.velocity.x == 0
            && movableObject.position.y == 0
            && Time.time - lastSlideFinishTime > cooldown
            && !(slideBehaviour && slideBehaviour.slide)
            && !(knockbackBehaviour && knockbackBehaviour.knockback)
            && !(stunBehaviour && stunBehaviour.stun)
            && !(attackManager && attackManager.attacking);
    }

    public void Dodge()
    {
        if (!CanDodge())
        {
            return;
        }
        if (walkBehaviour)
        {
            walkBehaviour.Stop();
        }
        anticipating = true;
        float dodgeDirection = Mathf.Sign(movableObject.velocity.z);
        movableObject.acceleration = Vector3.zero;
        movableObject.velocity = Vector3.zero;
        onAnticipate?.Invoke();
        anticipateEvent = eventManager.StartTimeout(() =>
        {
            anticipating = false;
            movableObject.UpdatePosition(movableObject.position + dodgeDirection * dodgeDistance * Vector3.forward);
            onDodge?.Invoke();
            recovering = true;
            recoverEvent = eventManager.StartTimeout(() =>
            {
                recovering = false;
                onRecover?.Invoke();
                onStop?.Invoke();
            }, recoveryTime);
        }, anticipateTime);
    }

    public void Stop()
    {
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
        onStop?.Invoke();
    }
}
