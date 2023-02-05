using ExtEvents;
using UnityEngine;

/// <summary>
/// Behaviours that handles death of character
/// </summary>
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

    /// <value>
    /// Invoked when character dies
    /// </value>
    [SerializeField] public ExtEvent onDie;

    private void Start()
    {
        var healthSystem = GetBehaviour<HealthSystem>();
        InvokeWhen(() => !healthSystem.Alive, Kill);
    }

    private void Kill()
    {
        DisableBehaviours(typeof(CharacterController));
        StopBehaviours(typeof(IEffect), typeof(IControlledBehaviour), typeof(StunBehaviour));

        void KillAfterKnockback()
        {
            Animator.SetBool(DeadParameter, true);
            onDie.Invoke();
            if (destroyOnDeath)
            {
                Destroy(Entity.gameObject, deathAnimationDuration);
            }
        }

        if (IsPlaying<KnockbackBehaviour>())
        {
            GetBehaviour<KnockbackBehaviour>().PlayEvents.onStop += KillAfterKnockback;
        }
        else
        {
            KillAfterKnockback();
        }
    }
}