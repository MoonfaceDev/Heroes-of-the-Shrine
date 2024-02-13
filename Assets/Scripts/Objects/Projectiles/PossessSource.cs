using UnityEngine;

/// <summary>
/// Object has two phases: warning and active. If a <see cref="IHittable"/> touches it during the active phase, it will
/// receive the possessed effect
/// </summary>
public class PossessSource : EntityBehaviour
{
    public HitSource hitSource;

    /// <value>
    /// Related animator
    /// </value>
    public Animator animator;

    /// <value>
    /// Time before the possess source is destroyed after hitting 
    /// </value>
    public float hitAnimationDuration;

    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Hit = Animator.StringToHash("Hit");

    /// <summary>
    /// Starts the source
    /// </summary>
    /// <param name="warningDuration">Duration of the warning phase</param>
    /// <param name="activeDuration">Duration of the active phase</param>
    /// <param name="relatedAttack">Attack that created the possess source</param>
    public void Activate(float warningDuration, float activeDuration, BaseAttack relatedAttack)
    {
        animator.SetBool(Warning, true);
        eventManager.StartTimeout(() =>
        {
            animator.SetBool(Warning, false);
            animator.SetBool(Active, true);

            var destroyTimeout = eventManager.StartTimeout(() =>
            {
                hitSource.Stop();
                animator.SetBool(Active, false);
                Destroy(gameObject);
            }, activeDuration);

            hitSource.Start(relatedAttack, _ =>
            {
                hitSource.Stop();
                animator.SetBool(Hit, true);
                Destroy(gameObject, hitAnimationDuration);
                eventManager.Cancel(destroyTimeout);
            });
        }, warningDuration);
    }
}