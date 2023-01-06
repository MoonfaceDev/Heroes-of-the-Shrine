using System.Collections.Generic;
using UnityEngine;

public class PossessSource : BaseComponent
{
    public BaseHitDetector hitDetector;
    public Animator animator;
    public float hitAnimationDuration;

    private string destroyTimeout;

    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Hit = Animator.StringToHash("Hit");

    public void Activate(float warningDuration, float activeDuration, List<string> hittableTags, float effectDuration,
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
                    possessedEffect.Play(effectDuration);
                }
            }, hittableTags);
        }, warningDuration);
    }
}