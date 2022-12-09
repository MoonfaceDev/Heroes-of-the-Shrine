using System.Collections.Generic;
using UnityEngine;

public class PossesSource : MonoBehaviour
{
    public BaseHitDetector hitDetector;
    public Animator animator;
    public float hitAnimationDuration;

    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Hit = Animator.StringToHash("Hit");

    public void Activate(float warningDuration, float activeDuration, List<string> hittableTags, float effectDuration)
    {
        animator.SetBool(Warning, true);
        EventManager.Instance.StartTimeout(() =>
        {
            animator.SetBool(Warning, false);
            animator.SetBool(Active, true);
            hitDetector.StartDetector(hittable =>
            {
                hitDetector.StopDetector();
                animator.SetBool(Hit, true);
                Destroy(gameObject, hitAnimationDuration);

                var possessedEffect = hittable.Character.GetComponent<PossessedEffect>();
                if (possessedEffect)
                {
                    possessedEffect.Play(effectDuration);
                }
            }, hittableTags);
            EventManager.Instance.StartTimeout(() =>
            {
                hitDetector.StopDetector();
                animator.SetBool(Active, false);
                Destroy(gameObject);
            }, activeDuration);
        }, warningDuration);
    }
}