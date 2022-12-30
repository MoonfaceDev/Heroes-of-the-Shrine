using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(HittableBehaviour))]
public class DieBehaviour : CharacterBehaviour
{
    [FormerlySerializedAs("destoryOnDeath")] public bool destroyOnDeath = true;
    public float deathAnimationDuration;
    
    private static readonly int DeadParameter = Animator.StringToHash("dead");

    public UnityEvent onDie;

    private void Start()
    {
        var hittableBehaviour = GetComponent<HittableBehaviour>();
        var healthSystem = GetComponent<HealthSystem>();

        hittableBehaviour.OnHit += _ =>
        {
            if (!healthSystem.Alive)
            {
                Kill();
            }
        };
    }

    public void Kill()
    {
        DisableBehaviours(typeof(BaseEffect), typeof(CharacterController));
        StopBehaviours(typeof(BaseEffect), typeof(BaseMovementBehaviour), typeof(AttackManager), typeof(StunBehaviour));

        void KillAfterKnockback()
        {
            Animator.SetBool(DeadParameter, true);
            onDie.Invoke();
            if (destroyOnDeath)
            {
                Destroy(gameObject, deathAnimationDuration);
            }
        }

        if (IsPlaying(typeof(KnockbackBehaviour)))
        {
            GetComponent<KnockbackBehaviour>().onStop.AddListener(KillAfterKnockback);
        }
        else
        {
            KillAfterKnockback();
        }
    }
}
