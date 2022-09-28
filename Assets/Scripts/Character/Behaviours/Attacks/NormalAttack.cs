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
        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            if (walkBehaviour)
            {
                walkBehaviour.Stop(true);
            }
        };
        onStart += () => hitDetector.Start();
        void StopHitDetector() => hitDetector.Stop();
        onFinish += StopHitDetector;
        onStop += StopHitDetector;
    }
}
