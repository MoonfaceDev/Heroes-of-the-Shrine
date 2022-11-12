using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(HittableBehaviour))]
public class DieBehaviour : CharacterBehaviour
{
    [FormerlySerializedAs("destoryOnDeath")] public bool destroyOnDeath = true;
    public float deathAnimationDuration;
    
    private static readonly int DeadParameter = Animator.StringToHash("dead");

    public event Action OnDie;

    private void Start()
    {
        var hittableBehaviour = GetComponent<HittableBehaviour>();
        var healthSystem = GetComponent<HealthSystem>();

        hittableBehaviour.OnHit += _ =>
        {
            if (healthSystem.health <= 0)
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
            OnDie?.Invoke();
            Animator.SetBool(DeadParameter, true);
            StopBehaviours(typeof(BaseEffect));
            if (destroyOnDeath)
            {
                Destroy(gameObject, deathAnimationDuration);
            }
        }

        if (IsPlaying(typeof(KnockbackBehaviour)))
        {
            GetComponent<KnockbackBehaviour>().OnStop += KillAfterKnockback;
        }
        else
        {
            KillAfterKnockback();
        }
    }
}
