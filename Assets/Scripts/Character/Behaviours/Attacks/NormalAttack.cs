using System;

public class NormalAttack : SimpleAttack
{
    public Hitbox hitbox;

    public override void Awake()
    {
        base.Awake();
        SingleHitDetector hitDetector = new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (hittableBehaviour)
            {
                HitCallable(hittableBehaviour);
            }
        });
        onStart += () => hitDetector.Start();
        void StopHitDetector() => hitDetector.Stop();
        onFinish += StopHitDetector;
        onStop += StopHitDetector;
    }
}
