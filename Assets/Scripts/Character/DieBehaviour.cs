using ExtEvents;
using UnityEngine;

/// <summary>
/// Behaviours that handles death of character
/// </summary>
[RequireComponent(typeof(HealthSystem))]
public class DieBehaviour : CharacterBehaviour
{
    /// <value>
    /// If <c>true</c>, destroys the character after death animation is over
    /// </value>
    public bool destroyOnDeath = true;

    /// <value>
    /// Time before character is destroyed. Used only if <see cref="destroyOnDeath"/> is <c>true</c>.
    /// </value>
    public float deathAnimationDuration;

    private static readonly int DeadParameter = Animator.StringToHash("dead");
    private static readonly int RespawnParameter = Animator.StringToHash("respawn");

    /// <value>
    /// Invoked when character dies
    /// </value>
    [SerializeField] public ExtEvent onDie;

    private void Start()
    {
        var healthSystem = GetComponent<HealthSystem>();

        Register(() =>
        {
            if (!healthSystem.Alive)
            {
                Kill();
            }
        });
    }

    private void Kill()
    {
        DisableBehaviours(typeof(IEffect), typeof(CharacterController));
        StopBehaviours(typeof(IEffect), typeof(IMovementBehaviour), typeof(BaseAttack), typeof(StunBehaviour));

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

    /// <summary>
    /// Starts respawn animation
    /// </summary>
    public void Respawn()
    {
        Animator.SetBool(RespawnParameter, true);
    }
}