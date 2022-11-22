using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseHitDetector))]
public class PossesSource : MonoBehaviour
{
    public GameObject warningIndicator;
    public GameObject activeIndicator;

    public void Activate(float warningDuration, float activeDuration, List<string> hittableTags)
    {
        warningIndicator.SetActive(true);
        EventManager.Instance.StartTimeout(() =>
        {
            warningIndicator.SetActive(false);
            activeIndicator.SetActive(true);
            var hitDetector = GetComponent<BaseHitDetector>();
            hitDetector.StartDetector(hittable =>
            {
                // TODO: Apply the PossessedEffect
                print(hittable);
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