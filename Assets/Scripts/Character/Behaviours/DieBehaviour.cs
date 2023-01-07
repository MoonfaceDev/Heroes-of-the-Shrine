using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HittableBehaviour))]
public class DieBehaviour : CharacterBehaviour
{
    public bool destroyOnDeath = true;
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
        DisableBehaviours(typeof(IEffect), typeof(CharacterController));
        StopBehaviours(typeof(IEffect), typeof(IMovementBehaviour), typeof(BaseAttack),
            typeof(StunBehaviour));

        void KillAfterKnockback()
        {
            Animator.SetBool(DeadParameter, true);
            onDie.Invoke();
            if (destroyOnDeath)
            {
                Destroy(gameObject, deathAnimationDuration);
            }
        }

        if (IsPlaying<KnockbackBehaviour>())
        {
            GetComponent<KnockbackBehaviour>().PlayEvents.onStop.AddListener(KillAfterKnockback);
        }
        else
        {
            KillAfterKnockback();
        }
    }
}