using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class PossesSource : MonoBehaviour
{
    public GameObject warningIndicator;
    public GameObject activeIndicator;

    public void Activate(float warningDuration, float activeDuration)
    {
        warningIndicator.SetActive(true);
        EventManager.Instance.StartTimeout(() =>
        {
            warningIndicator.SetActive(false);
            activeIndicator.SetActive(true);
            var singleHitDetector = new SingleHitDetector(EventManager.Instance, GetComponent<Hitbox>(),
                hittable =>
                {
                    // TODO: Apply the PossessedEffect
                    print(hittable);
                }
            );
            singleHitDetector.Start();
            EventManager.Instance.StartTimeout(() =>
            {
                singleHitDetector.Stop();
                activeIndicator.SetActive(false);
                Destroy(gameObject);
            }, activeDuration);
        }, warningDuration);
    }
}