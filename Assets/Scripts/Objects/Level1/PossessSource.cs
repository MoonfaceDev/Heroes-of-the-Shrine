using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessSource : MonoBehaviour
{
    public BaseHitDetector hitDetector;
    public Animator animator;
    public float hitAnimationDuration;

    private EventListener activateEvent;
    private EventListener destroyEvent;

    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Hit = Animator.StringToHash("Hit");

    public void Activate(float warningDuration, float activeDuration, List<string> hittableTags, float effectDuration,
        int hitDamage)
    {
        animator.SetBool(Warning, true);
        activateEvent = EventManager.Instance.StartTimeout(() =>
        {
            animator.SetBool(Warning, false);
            animator.SetBool(Active, true);

            destroyEvent = EventManager.Instance.StartTimeout(() =>
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
                EventManager.Instance.Detach(destroyEvent);

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

    public void Stop()
    {
        EventManager.Instance.Detach(activateEvent);
        EventManager.Instance.Detach(destroyEvent);
    }

    private void OnDestroy()
    {
        Stop();
    }
}