using System;
using UnityEngine;

[RequireComponent(typeof(HittableBehaviour))]
public class DieBehaviour : CharacterBehaviour
{
    public bool destoryOnDeath = true;
    public float deathAnimationDuration;

    public event Action OnDie;

    private void Start()
    {
        HittableBehaviour hittableBehaviour = GetComponent<HittableBehaviour>();
        HealthSystem healthSystem = GetComponent<HealthSystem>();

        hittableBehaviour.OnHit += (float damage) =>
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
            Animator.SetBool("dead", true);
            StopBehaviours(typeof(BaseEffect));
            if (destoryOnDeath)
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
