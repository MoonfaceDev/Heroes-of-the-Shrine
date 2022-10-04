using System.Collections;
using System.Collections.Generic;
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

        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop(true);
        };
    }

    protected override void CreateHitDetector()
    {
        hitDetector1 = CreateOneHitDetector(hitbox1);
        hitDetector2 = CreateOneHitDetector(hitbox2);
        onStart += () =>
        {
            hitDetector1.Start();
            StartCoroutine(DisableHitDetector1());
            StartCoroutine(EnableHitDetector2());
        };

        void StopDetectors()
        {
            hitDetector1.Stop();
            hitDetector2.Stop();
        }

        onFinish += StopDetectors;

        onStop += StopDetectors;
    }

    private SingleHitDetector CreateOneHitDetector(Hitbox hitbox)
    {
        return new(eventManager, hitbox, (hittable) =>
        {
            if (HittableTags.Contains(hittable.tag))
            {
                HitCallable(hittable);
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
