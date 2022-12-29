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
        DisableBehaviours(typeof(KnockbackBehaviour), typeof(BaseEffect));
        Type[] behavioursToStop = { typeof(BaseMovementBehaviour), typeof(AttackManager), typeof(StunBehaviour) };
        DisableBehaviours(behavioursToStop);
        StopBehaviours(behavioursToStop);

        void KillAfterKnockback()
        {
            Animator.SetBool(DeadParameter, true);
            StopBehaviours(typeof(BaseEffect));
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
