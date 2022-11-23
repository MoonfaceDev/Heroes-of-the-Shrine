using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseHitDetector))]
public class PossesSource : MonoBehaviour
{
    public GameObject warningIndicator;
    public GameObject activeIndicator;

    public void Activate(float warningDuration, float activeDuration, List<string> hittableTags, float effectDuration)
    {
        warningIndicator.SetActive(true);
        EventManager.Instance.StartTimeout(() =>
        {
            warningIndicator.SetActive(false);
            activeIndicator.SetActive(true);
            var hitDetector = GetComponent<BaseHitDetector>();
            hitDetector.StartDetector(hittable =>
            {
                var possessedEffect = hittable.Character.GetComponent<PossessedEffect>();
                if (possessedEffect)
                {
                    possessedEffect.Play(effectDuration);
                }
            }, hittableTags);
            EventManager.Instance.StartTimeout(() =>
            {
                hitDetector.StopDetector();
                activeIndicator.SetActive(false);
                Destroy(gameObject);
            }, activeDuration);
        }, warningDuration);
    }
}