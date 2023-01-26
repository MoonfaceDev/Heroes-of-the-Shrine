using ExtEvents;
using UnityEngine;

[RequireComponent(typeof(HittableBehaviour))]
public class DieBehaviour : CharacterBehaviour
{
    public bool destroyOnDeath = true;
    public float deathAnimationDuration;

    private static readonly int DeadParameter = Animator.StringToHash("dead");
    private static readonly int RespawnParameter = Animator.StringToHash("respawn");

    [SerializeField] public ExtEvent onDie;

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
            GetComponent<KnockbackBehaviour>().PlayEvents.onStop += KillAfterKnockback;
        }
        else
        {
            KillAfterKnockback();
        }
    }

    public void Respawn()
    {
        Animator.SetBool(RespawnParameter, true);
    }
}