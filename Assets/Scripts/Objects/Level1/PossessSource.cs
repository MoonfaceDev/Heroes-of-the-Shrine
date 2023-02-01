using UnityEngine;

/// <summary>
/// Object has two phases: warning and active. If a <see cref="IHittable"/> touches it during the active phase, it will
/// receive the possessed effect
/// </summary>
public class PossessSource : BaseComponent
{
    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    /// <value>
    /// Related animator
    /// </value>
    public Animator animator;
    
    /// <value>
    /// Time before the possess source is destroyed after hitting 
    /// </value>
    public float hitAnimationDuration;

    private string destroyTimeout;

    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Hit = Animator.StringToHash("Hit");

    /// <summary>
    /// Starts the source
    /// </summary>
    /// <param name="warningDuration">Duration of the warning phase</param>
    /// <param name="activeDuration">Duration of the active phase</param>
    /// <param name="hittableTags">Tags of objects that can get hit. Get it from <see cref="AttackManager.hittableTags"/></param>
    /// <param name="effectDuration">Duration of the applied possess effect on hit</param>
    /// <param name="hitDamage">Damage on hit</param>
    public void Activate(float warningDuration, float activeDuration, Tags hittableTags, float effectDuration,
        int hitDamage)
    {
        animator.SetBool(Warning, true);
        StartTimeout(() =>
        {
            animator.SetBool(Warning, false);
            animator.SetBool(Active, true);

            destroyTimeout = StartTimeout(() =>
            {
                hitDetector.StopDetector();
                animator.SetBool(Active, false);
                Destroy(gameObject);
            }, activeDuration);

            hitDetector.StartDetector(hittable =>
            {
                hitDetector.StopDetector();
                animator.SetBool(Hit, true);
                Destroy(gameObject, hitAnimationDuration);
                Cancel(destroyTimeout);

                if (!hittable.CanGetHit()) return;
                hittable.Hit(hitDamage);
                var possessedEffect = hittable.Character.GetComponent<PossessedEffect>();
                if (possessedEffect)
                {
                    possessedEffect.Play(new PossessedEffect.Command(effectDuration));
                }
            }, hittableTags);
        }, warningDuration);
    }
}