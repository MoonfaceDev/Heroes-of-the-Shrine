using System.Collections;
using UnityEngine;

public class NormalAttack : SimpleAttack
{
    public float attackDuration;
    public Hitbox hitbox;

    private SingleHitDetector hitDetector;

    public override void Awake()
    {
        base.Awake();
        hitDetector = new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (hittableBehaviour)
            {
                HitCallable(hittableBehaviour);
            }
        });
    }

    protected override IEnumerator ActiveCoroutine()
    {
        hitDetector.Start();
        yield return new WaitForSeconds(attackDuration);
        hitDetector.Stop();
    }

    protected override bool CanAttack()
    {
        RunBehaviour runBehaviour = GetComponent<RunBehaviour>();
        return base.CanAttack() && !(runBehaviour && runBehaviour.run);
    }
}
