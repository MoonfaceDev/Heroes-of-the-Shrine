using System.Collections;
using UnityEngine;

public class SpinningSwordsAttack : SimpleAttack
{
    public Hitbox hitbox1;
    public Hitbox hitbox2;
    public float hitbox1FinishTime;
    public float hitbox2StartTime;

    private SingleHitDetector hitDetector1;
    private SingleHitDetector hitDetector2;

    public override void Awake()
    {
        base.Awake();

        hitDetector1 = CreateHitDetector(hitbox1);
        hitDetector2 = CreateHitDetector(hitbox2);

        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop(true);
        };

        onStart += () =>
        {
            hitDetector1.Start();
            StartCoroutine(DisableHitDetector1());
            StartCoroutine(EnableHitDetector2());
        };

        void StopDetectors() {
            hitDetector1.Stop();
            hitDetector2.Stop();
        }

        onFinish += StopDetectors;

        onStop += StopDetectors;
    }

    private SingleHitDetector CreateHitDetector(Hitbox hitbox)
    {
        return new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (hittableBehaviour)
            {
                HitCallable(hittableBehaviour);
            }
        });
    }

    private IEnumerator DisableHitDetector1()
    {
        yield return new WaitForSeconds(hitbox1FinishTime);
        hitDetector1.Stop();
    }

    private IEnumerator EnableHitDetector2()
    {
        yield return new WaitForSeconds(hitbox2StartTime);
        hitDetector2.Start();
    }
}
