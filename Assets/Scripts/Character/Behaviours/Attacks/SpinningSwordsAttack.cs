using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningSwordsAttack : NormalAttack
{
    public Hitbox hitbox1;
    public Hitbox hitbox2;
    public float hitbox1FinishTime;
    public float hitbox2StartTime;

    private SingleHitDetector hitDetector1;
    private SingleHitDetector hitDetector2;

    protected override void CreateHitDetector()
    {
        hitDetector1 = CreateOneHitDetector(hitbox1);
        hitDetector2 = CreateOneHitDetector(hitbox2);
        Coroutine disableHitDetector1 = null;
        Coroutine enableHitDetector2 = null;
        onStart += () =>
        {
            hitDetector1.Start();
            disableHitDetector1 = StartCoroutine(DisableHitDetector1());
            enableHitDetector2 = StartCoroutine(EnableHitDetector2());
        };

        onFinish += () => 
        {
            hitDetector1.Stop();
            hitDetector2.Stop();
        };

        onStop += () =>
        {
            StopCoroutine(disableHitDetector1);
            StopCoroutine(enableHitDetector2);
        };
    }

    private SingleHitDetector CreateOneHitDetector(Hitbox hitbox)
    {
        return new(eventManager, hitbox, (hittable) =>
        {
            if (IsHittableTag(hittable.tag))
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
